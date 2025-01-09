using CommunityManager.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace CommunityManager.Pages.Residents
{
    public class UpdateLoginModel : PageModel
    {
        private readonly UserManager<CommunityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly CommunityContext _context;

        public UpdateLoginModel(CommunityContext context, UserManager<CommunityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [BindProperty]
        [Required]
        public string Username { get; set; }

        [BindProperty]
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [BindProperty]
        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string ErrorMessage { get; set; }

        public string name = "";

        public int id;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var resident = await _context.Residents
                .Include(r => r.house)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (resident == null)
            {
                return NotFound();
            }
            name = resident.name;
            this.id = (int)id;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var users = await _userManager.Users.ToListAsync();
            CommunityUser? user = null;

            foreach (var u in users)
            {
                if (u.ResidentID == id)
                {
                    user = u;
                    break;
                }
            }
            
            if(user != null)
            {
                var a = await _userManager.RemovePasswordAsync(user);
                var b = await _userManager.AddPasswordAsync(user, Password);
                var c = await _userManager.SetUserNameAsync(user, Username);

                if (a.Succeeded && b.Succeeded && c.Succeeded)
                {
                    return RedirectToPage("/Residents/Index");
                }
                else
                {
                    ErrorMessage = "There was an error creating your account.";
                    foreach (var error in a.Errors)
                    {
                        ErrorMessage += $" {error.Description}";
                    }
                    return Page();
                }
            }else
            {
                ErrorMessage = "No user was found for account";
                return Page();
            }
        }
    }
}
