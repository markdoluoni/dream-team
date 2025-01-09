using CommunityManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CommunityManager.Pages.Residents
{
    [Authorize]
    public class BalanceModel : PageModel
    {
        public float amount { get; set; }

        public void OnGet(int amount)
        {
            this.amount = amount;
        }
    }
    [Authorize]
    public class SuccessPaymentModel : PageModel
    {
        private readonly CommunityContext _context;

        public SuccessPaymentModel(CommunityContext context)
        {
            _context = context;
        }

        public decimal amount { get; set; }

        [BindProperty]
        public Resident resident { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id, decimal amount)
        {
            amount = Math.Round(amount, 2);
            this.amount = amount;

            if (id == null)
            {
                return NotFound();
            }

            Resident? newResident = await _context.Residents
                .Include(resident => resident.house)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (newResident == null)
            {
                return NotFound();
            }
            resident = newResident;

            if (resident.rentDue == null)
            {
                resident.rentDue = -amount;
            }
            else
            {
                resident.rentDue -= amount;
            }

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

            return Page();
        }

        private bool ResidentExists(int? id)
        {
            return _context.Residents.Any(e => e.Id == id);
        }
    }
}
