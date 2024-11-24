using System.ComponentModel.DataAnnotations;

namespace MusicMatch.Models
{
    public class EventAttendee
    {
        public int Id { get; set; }

        [Required]
        public int EventId { get; set; }
        public virtual Event? Event { get; set; }

        [Required]
        public string UserId { get; set; }
        public virtual ApplicationUser? ApplicationUser { get; set; }

        [Required]
        public string RSVP_Status { get; set; } //Going Maybe/Interested Not Going
    }
}
