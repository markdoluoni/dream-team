using CommunityManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace CommunityManager.Pages.Residents
{
    [Authorize(Roles = "Manager")]
    public class CreateModel : PageModel
    {
        private readonly CommunityContext _context;

        public CreateModel(CommunityContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Resident resident { get; set; } = default!;

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Residents.Add(resident);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
