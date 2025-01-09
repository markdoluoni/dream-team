using CommunityManager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Stripe.Checkout;
using Stripe;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;

namespace CommunityManager.Pages.Residents
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
        public Resident resident { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var newResident = await _context.Residents
                .Include(resident => resident.house)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (newResident == null)
            {
                return NotFound();
            }
            resident = newResident;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var houseFromDb = await _context.Houses
                .Include(h => h.residents)
                .FirstOrDefaultAsync(h => h.Id == 1);

            //resident.house = houseFromDb;
            _context.Attach(resident).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ResidentExists(resident.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Details", new { id = resident.Id });
        }

        private bool ResidentExists(int? id)
        {
            return _context.Residents.Any(e => e.Id == id);
        }
    }
}
