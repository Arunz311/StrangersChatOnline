using Microsoft.AspNetCore.Mvc.RazorPages;

namespace StrangersChat2.Areas.Identity.Pages.Account
{
    public class ChatPageModel : PageModel
    {
        public string PreferredName { get; set; } = "";
        public string Gender { get; set; } = "";

        public void OnGet()
        {
            PreferredName = TempData["PreferredName"] as string;
            Gender = TempData["Gender"] as string;
        }
    }
}
