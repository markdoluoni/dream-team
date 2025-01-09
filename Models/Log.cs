// Models/Log.cs
using System;
using System.ComponentModel.DataAnnotations;

namespace CommunityManager.Models
{
    public class Log
    {
        [Key]
        public int? Id { get; set; }

        public int? ResidentId { get; set; }

        [Required]
        public string? Action { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime Timestamp { get; set; } = DateTime.Now;

        public string? Description { get; set; }

        // Navigation property for Resident
        public Resident? Resident { get; set; }
    }
}
