using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace StrangersChat2.Pages
{
    public class HomePageAfterLoginModel : PageModel
    {
        [BindProperty]
        public string PreferredName { get; set; } = "";

        [BindProperty]
        public string Gender { get; set; } = "";

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            TempData["PreferredName"] = PreferredName;
            TempData["Gender"] = Gender;

            return RedirectToPage("/Account/ChatPage", new { area = "Identity" });
        }
    }
}
