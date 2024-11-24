using System.ComponentModel.DataAnnotations;

namespace MusicMatch.Models
{
    public class UserMood
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }

        [Required]
        public int MoodId {  get; set; }
        public virtual Mood? Mood { get; set; }

        [Required]
        public DateTime Timestamp { get; set; }
    }
}
