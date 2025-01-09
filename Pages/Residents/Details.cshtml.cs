using CommunityManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;
using System.ComponentModel.DataAnnotations;

namespace CommunityManager.Pages.Residents
{
    [Authorize]
    public class DetailsModel : PageModel
    {
        private readonly UserManager<CommunityUser> _userManager;
        private readonly CommunityContext _context;
        private readonly StripeSettings _stripeSettings;

        public DetailsModel(CommunityContext context, UserManager<CommunityUser> userManager, IOptions<StripeSettings> stripeSettings)
        {
            _userManager = userManager;
            _context = context;
            _stripeSettings = stripeSettings.Value;
        }

        [BindProperty]
        public Resident resident { get; set; } = default!;

        public string houseName = "-";

        [DataType(DataType.Currency)]
        [BindProperty]
        public decimal? payment { get; set; } = null;

        public List<Log> Logs { get; set; } = new List<Log>();

        public bool userExists = false;

        public async Task<IActionResult> OnGetAsync(int? id)
        {


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

            if (resident.house != null) houseName = resident.house.name;

            Logs = await _context.Logs
                .Where(log => log.ResidentId == id)
                .OrderByDescending(log => log.Timestamp)
            .ToListAsync();

            var users = await _userManager.Users.ToListAsync();

            foreach (var user in users)
            {
                if(user.ResidentID == resident.Id)
                {
                    userExists = true;
                    break;
                }
            }

            return Page();
        }

        public async Task<IActionResult> OnPostPayAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var currency = "usd";
            var baseUrl = new Uri($"{Request.Scheme}://{Request.Host}").AbsoluteUri;
            var successUrl = baseUrl + "Residents/SuccessPayment?id=" + resident.Id.ToString() + "&amount=" + payment;
            var cancelUrl = baseUrl + "Residents/Details?id=" + resident.Id.ToString();
            StripeConfiguration.ApiKey = _stripeSettings.SecretKey;

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string>
                {
                    "card"
                },
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            Currency = currency,
                            UnitAmount = Convert.ToInt32(payment) * 100,
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = "Rent Payment",
                                Description = "Payment for " + resident.name + " balance of $" + resident.rentDue.ToString()
                            }
                        },
                        Quantity = 1
                    }
                },
                Mode = "payment",
                SuccessUrl = successUrl,
                CancelUrl = cancelUrl
            };

            var service = new SessionService();
            var session = service.Create(options);

            await LogAction(resident.Id, "Payment", $"Resident paid ${payment} towards rent.");

            return Redirect(session.Url);
        }

        public async Task<IActionResult> OnPostChargeAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (resident.rentDue == null)
            {
                resident.rentDue = payment;
            }
            else
            {
                resident.rentDue += payment;
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

            await LogAction(resident.Id, "Charge", $"Manager charged ${payment} to the resident's account.");

            return RedirectToPage("./Details", new { id = resident.Id });
        }
        private async Task LogAction(int? residentId, string action, string description)
        {
            var log = new Log
            {
                ResidentId = residentId,
                Action = action,
                Description = description,
                Timestamp = DateTime.Now
            };
            _context.Logs.Add(log);
            await _context.SaveChangesAsync();
        }
        private bool ResidentExists(int? id)
        {
            return _context.Residents.Any(e => e.Id == id);
        }
    }
}
