using CommunityManager.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace CommunityManager.Pages.Residents
{
    public class UserModel : PageModel
    {
        private readonly UserManager<CommunityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly CommunityContext _context;

        public UserModel(CommunityContext context, UserManager<CommunityUser> userManager, RoleManager<IdentityRole> roleManager)
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

            

            var user = new CommunityUser { UserName = Username };
            user.ResidentID = id;
            var result = await _userManager.CreateAsync(user, Password);

            if (result.Succeeded)
            {
                _userManager.AddToRoleAsync(user, "Resident");
                return RedirectToPage("/Residents/Index"); //Redirect to home page
            }
            else
            {
                ErrorMessage = "There was an error creating your account.";
                foreach (var error in result.Errors)
                {
                    ErrorMessage += $" {error.Description}";
                }
                return Page();
            }
        }
    }
}
