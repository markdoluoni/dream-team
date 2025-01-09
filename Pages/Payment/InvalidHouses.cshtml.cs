using CommunityManager.Models;
using CommunityManager.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CommunityManager.Pages.Payment
{
    [Authorize(Roles = "Manager")]
    public class InvalidHousesModel : PageModel
    {
        private readonly CommunityContext _context;

        public InvalidHousesModel(CommunityContext context)
        {
            _context = context;
        }

        public IList<Resident> Residents { get; set; } = default!;

        public async Task OnGetAsync()
        {
            IList<Resident> unsortedResidents = await _context.Residents
                .Include(r => r.house)
                .ToListAsync();

            Residents = new List<Resident>();

            foreach(Resident res in unsortedResidents)
            {
                if(res.house == null)
                {
                    Residents.Add(res);
                }
            }
        }
    }
}
