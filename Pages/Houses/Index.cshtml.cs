using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CommunityManager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace CommunityManager.Pages.Houses
{
    [Authorize(Roles = "Manager")]
    public class IndexModel : PageModel
    {
        private readonly CommunityContext _context;

        public IndexModel(CommunityContext context)
        {
            _context = context;
        }

        public IList<House> Houses { get;set; } = default!;

        public Resident resident { get; set; } = default!;

        public async Task OnGetAsync()
        {
            Houses = await _context.Houses.Include(h => h.residents).ToListAsync();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var house = await _context.Houses
                .Include(h => h.residents)
                .FirstOrDefaultAsync(h => h.Id == id);

            if (house == null)
            {
                return NotFound();
            }

            if(house.residents != null) house.residents.Clear();
            _context.Houses.Remove(house);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
