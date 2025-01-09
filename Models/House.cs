using System.ComponentModel.DataAnnotations;

namespace CommunityManager.Models
{
    public class House
    {
        public int? Id { get; set; }

        [Display(Name = "Name")]
        public required string name { get; set; }

        [Display(Name = "Address")]
        [DisplayFormat(NullDisplayText = "-")]
        public string? address { get; set;}

        [Display(Name = "Utility")]
        public decimal utility { get; set; } = 0;

        public List<Resident>? residents { get; set; }
    }
}
