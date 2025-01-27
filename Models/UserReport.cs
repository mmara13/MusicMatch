using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MusicMatch.Models
{
    public class UserReport
    {
       
            [Key]
            public int Id { get; set; }

            [Required]
            [StringLength(500, ErrorMessage = "The reason cannot exceed 500 characters.")]
            public string Reason { get; set; }

            [Required]
            public DateTime ReportedAt { get; set; } = DateTime.UtcNow;

            // Foreign key for the reported user
            [Required]
            public string ReportedUserId { get; set; }
            public bool IsResolved { get; set; }

            [ForeignKey("ReportedUserId")]
            public virtual ApplicationUser ReportedUser { get; set; }

            // Foreign key for the user who made the report
            [Required]
            public string ReportedById { get; set; }

            [ForeignKey("ReportedById")]
            public virtual ApplicationUser ReportedBy { get; set; }

            
    }
}



