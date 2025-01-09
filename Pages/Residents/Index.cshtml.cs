using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CommunityManager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace CommunityManager.Pages.Residents
{
    [Authorize(Roles = "Manager")]
    public class IndexModel : PageModel
    {
        private readonly CommunityContext _context;
        private readonly UserManager<CommunityUser> _userManager;

        public IndexModel(CommunityContext context, UserManager<CommunityUser> userManager)
        {
            _userManager = userManager;
            _context = context;
        }

        public IList<Resident> Residents { get; set; } = default!;

        public async Task OnGetAsync()
        {

            Residents = await _context.Residents
                .Include(resident => resident.house)
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var Logs = await _context.Logs
                .Where(log => log.ResidentId == id)
                .OrderByDescending(log => log.Timestamp)
            .ToListAsync();

            _context.Logs.RemoveRange(Logs);
            await _context.SaveChangesAsync();

            var resident = await _context.Residents.FindAsync(id);
            if (resident != null)
            {
                _context.Residents.Remove(resident);
                await _context.SaveChangesAsync();
            }

            var users = await _userManager.Users.ToListAsync();

            foreach (var user in users)
            {
                if (user.ResidentID == resident.Id)
                {
                    await _userManager.DeleteAsync(user);
                    break;
                }
            }

            return RedirectToPage("./Index");
        }
    }
}
