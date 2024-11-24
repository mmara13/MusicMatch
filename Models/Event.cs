using System.ComponentModel.DataAnnotations;

namespace MusicMatch.Models
{
    public class Event
    {
        public int Id { get; set; }
        [Required]
        [StringLength(200)]
        public string Name { get; set; }
        public string? Description { get; set; }
        [Required]
        public DateTime DateTime { get; set; }

        [Required]
        public string Location { get; set; }

        [Required]
        public string Type { get; set; } //Concert /festival

        public int? ArtistId { get; set; }
        
        public virtual Artist? Artist { get; set; }

        public virtual ICollection<EventAttendee>? Attendees { get; set; }
        public virtual ICollection<ChatRoom>? ChatRooms { get; set; }
    }
}
