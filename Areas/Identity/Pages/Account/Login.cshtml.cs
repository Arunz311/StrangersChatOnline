using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using StrangersChat2.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace StrangersChat2.Areas.Identity.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;
        private readonly UserManager<ApplicationUser> _userManager; // Add UserManager dependency

        // Constructor with dependencies injected
        public LoginModel(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, ILogger<LoginModel> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager; // Initialize UserManager
            _logger = logger;
        }

        // BindProperty for inputs in the login form
        [BindProperty]
        public InputModel Input { get; set; }

        // Properties for login form inputs
        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        // Handles POST request for login
        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/HomePageAfterLogin");

            // Authenticate user with provided credentials
            var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                _logger.LogInformation("User logged in.");
                return LocalRedirect(returnUrl);
            }
            if (result.RequiresTwoFactor)
            {
                return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
            }
            if (result.IsLockedOut)
            {
                _logger.LogWarning("User account locked out.");
                return RedirectToPage("./Lockout");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return Page();

            }

        }

        // Handles POST request for forgot password
        public async Task<IActionResult> OnPostForgotPasswordAsync()
        {
            if (ModelState.IsValid)
            {
                // Find user by email address
                var user = await _userManager.FindByEmailAsync(Input.Email);

                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return RedirectToPage("./ForgotPasswordConfirmation");
                }

                // Generate password reset token
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                // Construct callback URL for password reset
                var callbackUrl = Url.Page(
                    "/Account/ResetPassword",
                    pageHandler: null,
                    values: new { code = token },
                    protocol: Request.Scheme);

                // Send reset password email
                await SendResetPasswordEmailAsync(Input.Email, "Reset Password", $"Please reset your password by <a href='{callbackUrl}'>clicking here</a>.");

                // Redirect to forgot password confirmation page
                return RedirectToPage("./ForgotPasswordConfirmation");
            }

            // If model state is not valid, redisplay form
            return Page();
        }

        // Method to send reset password email
        private async Task SendResetPasswordEmailAsync(string email, string subject, string htmlMessage)
        {
            try
            {
                using (var message = new MailMessage())
                {
                    message.From = new MailAddress("arunzzspace@gmail.com");
                    message.To.Add(email);
                    message.Subject = subject;
                    message.IsBodyHtml = true;
                    message.Body = htmlMessage;

                    using (var smtpClient = new SmtpClient("smtp.gmail.com", 587))
                    {
                        smtpClient.EnableSsl = true;
                        smtpClient.UseDefaultCredentials = false;
                        smtpClient.Credentials = new NetworkCredential("arunzzspace@gmail.com", "gvgjjzlafmkpefkb");

                        await smtpClient.SendMailAsync(message);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending email.");
                throw;
            }
        }
    }
}
