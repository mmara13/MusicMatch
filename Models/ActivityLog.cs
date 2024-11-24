using System.ComponentModel.DataAnnotations;

namespace MusicMatch.Models
{
    public class ActivityLog
    {
        public int Id { get; set; }
        [Required]
        public string ActivityType { get; set; } //"Liked Song", "Created Playlist" etc

        [Required]
        public string ActivityDetails { get; set; }

        public DateTime Timestamp { get; set; }
        public string UserId { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
    }
}
