using System.ComponentModel.DataAnnotations;

namespace MusicMatch.Models
{
    public class UserSong
    {
        public int Id { get; set; }
        [Required]
        public int SongId { get; set; }
        public virtual Song? Song { get; set; }
        [Required]
        public string UserId { get; set; }
        public virtual ApplicationUser? ApplicationUser { get; set; }
    }
}
