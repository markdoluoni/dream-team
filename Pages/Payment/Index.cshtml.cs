using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CommunityManager.Models;
using Microsoft.AspNetCore.Mvc;
using CommunityManager.Service;

namespace CommunityManager.Pages.Payment
{
    public class IndexModel : PageModel
    {
        private readonly CommunityContext _context;
        private readonly IEmailService _emailService;

        public IndexModel(CommunityContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        public IList<Resident> Residents { get; set; } = default!;
        public string subject = getCurrentMonth() + " rent";
        public string body = "Hi <name>,\r\n" +
                                "I have charged you for " + getCurrentMonth() + " rent. Your rent amounts are <baseRent> in rent and <utilityShare> for utilities. Thats a total of <total>. The link to pay is:\r\n" +
                                "<link>\r\n" +
                                "Thanks and Blessings,\r\n" +
                                "Ted";
        public string footer = "Rev. Dr. Andrea LPC and Rev. Ted Godwin-Stremler\r\n" +
                                "New Revelations Collegiate Mission - Denton Texas\r\n" +
                                "915-471-4655\r\n" +
                                "915-471-5436";

        public async Task<IActionResult> OnGetAsync()
        {

            string baseUrl = new Uri($"{Request.Scheme}://{Request.Host}").AbsoluteUri;
            Residents = await _context.Residents
                .Include(resident => resident.house)
                .ToListAsync();

            bool eachResidentHasHouse = true;

            foreach (Resident resident in Residents)
            {
                if(resident.house == null)
                {
                    eachResidentHasHouse = false;
                    break;
                }
            }

            if (eachResidentHasHouse)
            {
                return Page();
            }
            else 
            {
                return Redirect(baseUrl + "Payment/InvalidHouses");
            }
        }

        public async Task<IActionResult> OnPostChargeAsync(int[] selectedResidents, string subject, string body, string footer)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            string baseUrl = new Uri($"{Request.Scheme}://{Request.Host}").AbsoluteUri;
            Residents = await _context.Residents
                .Include(resident => resident.house)
                .ToListAsync();
            
            for (int i = 0; i < Residents.Count; i++)
            {
                var house = Residents[i].house;

                if (selectedResidents.Contains(i))
                {
                    decimal charge = Residents[i].baseRent;
                    decimal utilityShare = 0;

                    if (house!.residents!.Count > 0) utilityShare = house.utility / house.residents!.Count;
                    charge += utilityShare;

                    Residents[i].rentDue += charge;

                    var logEntry = new Log
                    {
                        ResidentId = Residents[i].Id,
                        Timestamp = DateTime.Now,
                        Action = "Rent and Utility charge",
                        Description = $"Charged {charge:C} for rent and utilities."
                    };
                    _context.Logs.Add(logEntry);
                    //send email
                    if (Residents[i].email != null)
                    {
                        string name = Residents[i].name;
                        string baseRent = Residents[i].baseRent.ToString("C");
                        string utilityShareStr = utilityShare.ToString("C");
                        string total = (Residents[i].baseRent + utilityShare).ToString("C");
                        string link = baseUrl + "NewLogin/NewLogin";
                        string combinedBody = body + "<br/>--<br/>" + "<span style=\"opacity: 0.6;\">" + footer + "</span>";

                        string editedSubject = subject.Replace("<name>", name)
                            .Replace("<baseRent>", baseRent)
                            .Replace("<utilityShare>", utilityShareStr)
                            .Replace("<total>", total)
                            .Replace("<link>", link);
                        string editedBody = combinedBody.Replace("<name>", name)
                            .Replace("<baseRent>", baseRent)
                            .Replace("<utilityShare>", utilityShareStr)
                            .Replace("<total>", total)
                            .Replace("<link>", link)
                            .Replace("\r\n", "<br/>");

                        EmailMetaData emailMetadata = new(Residents[i].email!
                        , editedSubject
                        , editedBody);
                        await _emailService.Send(emailMetadata);
                    }

                    try
                    {
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!ResidentExists(Residents[i].Id))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                }
            }
            return Redirect(baseUrl + "Payment/SuccessCharge");
        }

        private bool ResidentExists(int? id)
        {
            return _context.Residents.Any(e => e.Id == id);
        }

        private static string getCurrentMonth()
        {
            int monthNumber = DateTime.Now.Month;
            string monthStr = "December";
            switch(monthNumber)
            {
                case 1: monthStr = "January"; break;
                case 2: monthStr = "February"; break;
                case 3: monthStr = "March"; break;
                case 4: monthStr = "April"; break;
                case 5: monthStr = "May"; break;
                case 6: monthStr = "June"; break;
                case 7: monthStr = "July"; break;
                case 8: monthStr = "August"; break;
                case 9: monthStr = "September"; break;
                case 10: monthStr = "October"; break;
                case 11: monthStr = "November"; break;
                case 12: monthStr = "December"; break;
                default: break;
            }
            return monthStr;
        }
    }
}
