using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using CommunityManager.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace CommunityManager.Pages.NewLogin
{
    public class Login : PageModel
    {
        private readonly SignInManager<CommunityUser> _signInManager;
        private readonly UserManager<CommunityUser> _userManager;


        [BindProperty]
        public string Username { get; set; } = string.Empty;

        [BindProperty]
        public string Password { get; set; } = string.Empty;

        public string? ErrorMessage { get; private set; }

        public Login(SignInManager<CommunityUser> signInManager, UserManager<CommunityUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public async Task<IActionResult> OnGet()
        {
            var a = User.Identity.Name;
            var b = _userManager.Users.Where(user => a == user.UserName).ToList();

            if(!b.IsNullOrEmpty())
            {
                var user = b.First();
                if(user.ResidentID == null)
                {
                    return RedirectToPage("/Residents/Index");
                }else
                {
                    string baseUrl = new Uri($"{Request.Scheme}://{Request.Host}").AbsoluteUri;
                    return Redirect(baseUrl + "Residents/Details?id=" + user.ResidentID.ToString());
                }
            }

            ErrorMessage = string.Empty;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.FindByNameAsync(Username);
            
            if (user == null)
            {
                ErrorMessage = "Invalid username or password.";
                return Page();
            }

            var result = await _signInManager.PasswordSignInAsync(user, Password, false, false);
            if (result.Succeeded)
            {
                //A manager has logged in, go to manager dash
                if(await _userManager.IsInRoleAsync(user, "Manager"))
                {
                    return RedirectToPage("/Residents/Index");
                }
                //A resident has logged in.
                else
                {
                    string baseUrl = new Uri($"{Request.Scheme}://{Request.Host}").AbsoluteUri;
                    return Redirect(baseUrl + "Residents/Details?id=" + user.ResidentID.ToString());
                }
            }
            else
            {
                ErrorMessage = "Invalid username or password.";
                return Page(); // Stay on login page if authentication fails
            }
        }

        public IActionResult OnGetLogout()
        {
            // Clear session or authentication
            HttpContext.Session.Clear();
            HttpContext.SignOutAsync();

            // Redirect to sign-in page
            return RedirectToPage("/NewLogin/NewLogin");
        }

    }
}
