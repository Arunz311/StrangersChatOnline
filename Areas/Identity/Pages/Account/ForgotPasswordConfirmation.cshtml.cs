using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using StrangersChat2.Models;
using System.Net.Mail;
using System.Net;
using System.Security.Policy;

public class ForgotPasswordConfirmationModel : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<ForgotPasswordConfirmationModel> _logger;

    public ForgotPasswordConfirmationModel(UserManager<ApplicationUser> userManager, ILogger<ForgotPasswordConfirmationModel> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<IActionResult> OnPostForgotPasswordAsync(string email)
    {
        if (!string.IsNullOrEmpty(email))
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.Page(
                    "/Account/ResetPassword",
                    pageHandler: null,
                    values: new { code = token },
                    protocol: Request.Scheme);

                // Send the reset password email
                await SendResetPasswordEmailAsync(email, "Reset Password", $"Please reset your password by <a href='{callbackUrl}'>clicking here</a>.");

                // Redirect to the reset password confirmation page
                return RedirectToPage("/Account/ResetPassword");
            }
        }

        // If user not found or email is empty, redirect back to login
        return RedirectToPage("/Account/Login");
    }

    // Method to send reset password email
    private async Task SendResetPasswordEmailAsync(string email, string subject, string htmlMessage)
    {
        // Implement your email sending logic here
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

