using CommunityManager.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CommunityManager.Pages.NewLogin
{
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<CommunityUser> _signInManager;

        public LogoutModel(SignInManager<CommunityUser> signInManager)
        {
            _signInManager = signInManager;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await _signInManager.SignOutAsync();
            return RedirectToPage("/NewLogin/NewLogin");
        }
    }
}
