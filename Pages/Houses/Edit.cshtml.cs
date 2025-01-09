using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CommunityManager.Models;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace CommunityManager.Pages.Houses
{
    [Authorize(Roles = "Manager")]
    public class EditModel : PageModel
    {
        private readonly CommunityContext _context;

        public EditModel(CommunityContext context)
        {
            _context = context;
        }

        [BindProperty]
        public House House { get; set; } = default!;

        [BindProperty]
        public List<int> SelectedResidentIds { get; set; } = new List<int>();

        public List<Resident> AllResidents { get; set; } = new List<Resident>();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Fetch house and associated residents
            var house = await _context.Houses
                .Include(h => h.residents)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (house == null)
            {
                return NotFound();
            }

            House = house;

            // Fetch all available residents for the dropdown
            AllResidents = await _context.Residents
                .Include(resident => resident.house)
                .ToListAsync();

            // Populate selected residents for pre-selection
            SelectedResidentIds = house.residents.Select(r => (int)r.Id).ToList();

            return Page();
        }
        
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                AllResidents = await _context.Residents
                    .Include(resident => resident.house)
                    .ToListAsync();
                return Page();
            }

            var houseFromDb = await _context.Houses
                .Include(h => h.residents)
                .FirstOrDefaultAsync(h => h.Id == House.Id);

            if (houseFromDb == null)
            {
                return NotFound();
            }

           
            houseFromDb.name = House.name;
            houseFromDb.address = House.address;
            houseFromDb.utility = House.utility;

            
            var selectedResidents = await _context.Residents
                .Where(r => SelectedResidentIds.Contains((int)r.Id))
                .Include(resident => resident.house)
                .ToListAsync();

            houseFromDb.residents.Clear();
            houseFromDb.residents.AddRange(selectedResidents);

            foreach (Resident resident in selectedResidents)
            {
                resident.house = houseFromDb;
                _context.Attach(resident).State = EntityState.Modified;
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HouseExists(House.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool HouseExists(int? id)
        {
            return _context.Houses.Any(e => e.Id == id);
        }
    }
}
