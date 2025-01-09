using System.ComponentModel.DataAnnotations;

namespace CommunityManager.Models
{
    public class Resident
    {
        public int? Id { get; set; }

        [Display(Name = "Name")]
        public required string name { get; set; }

        [Range(1,100)]
        [Display(Name = "Age")]
        [DisplayFormat(NullDisplayText = "-")]
        public int? age { get; set; }

        [Display(Name = "Base Rent")]
        public required decimal baseRent { get; set; }

        [Display(Name = "Rent Due")]
        [DisplayFormat(DataFormatString = "{0:C}", NullDisplayText = "-")]
        public decimal? rentDue { get; set; } = 0;
        
        [Phone]
        [Display(Name = "Phone Number")]
        [DisplayFormat(NullDisplayText = "-")]
        public string? phoneNumber { get; set; }

        [EmailAddress]
        [Display(Name = "Email")]
        [DisplayFormat(NullDisplayText = "-")]
        public string? email { get; set; }

        [Display(Name = "House")]
        [DisplayFormat(NullDisplayText = "-")]
        public House? house { get; set; }

        public bool charge { get; set; } = true;
    }
}