using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CommunityManager.Models;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace CommunityManager.Pages.Houses
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
        public House House { get; set; } = default!;

        [BindProperty]
        public List<int> SelectedResidentIds { get; set; } = new List<int>();

        public List<Resident> AllResidents { get; set; } = new List<Resident>();

        public async Task<IActionResult> OnGetAsync()
        {
            AllResidents = await _context.Residents
                .Include(resident => resident.house)
                .ToListAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                AllResidents = await _context.Residents
                    .Include(resident => resident.house)
                    .ToListAsync();
                return Page();
            }
            
            var selectedResidents = await _context.Residents
                .Where(r => SelectedResidentIds.Contains((int)r.Id))
                .ToListAsync();

            House.residents = selectedResidents;

            foreach (Resident resident in selectedResidents)
            {
                resident.house = House;
                _context.Attach(resident).State = EntityState.Modified;
            }

            _context.Houses.Add(House);
            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }
    }
}
