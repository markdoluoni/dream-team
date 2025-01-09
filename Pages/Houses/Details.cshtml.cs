using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CommunityManager.Models;
using Microsoft.AspNetCore.Authorization;

namespace CommunityManager.Pages.Houses
{
    [Authorize(Roles = "Manager")]
    public class DetailsModel : PageModel
    {
        private readonly CommunityContext _context;

        public DetailsModel(CommunityContext context)
        {
            _context = context;
        }

        public House House { get; set; } = default!;

        public Resident resident { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var house = await _context.Houses.Include(h => h.residents).FirstOrDefaultAsync(m => m.Id == id);
            if (house == null)
            {
                return NotFound();
            }
            else
            {
                House = house;
            }
            return Page();
        }
        public async Task<IActionResult> OnPostAsync(int? id, int? ResidentId)
        {
            if (id == null || ResidentId == null)
            {
                return NotFound();
            }

            var house = await _context.Houses.Include(h => h.residents).FirstOrDefaultAsync(m => m.Id == id);
            if (house == null)
            {
                return NotFound();
            }

            
            var residentToRemove = house.residents.FirstOrDefault(r => r.Id == ResidentId);
            if (residentToRemove != null)
            {
                house.residents.Remove(residentToRemove);
                _context.Residents.Remove(residentToRemove); 
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Details", new { id = house.Id });
        }
    }
}

