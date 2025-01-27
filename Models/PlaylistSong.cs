using System.ComponentModel.DataAnnotations;

namespace MusicMatch.Models
{
    public class PlaylistSong
    {
        public int Id { get; set; }

        [Required]  
        public int PlaylistId { get; set; }
        public virtual Playlist? Playlist { get; set; }

        [Required]
        public int SongId   { get; set; }
        public virtual Song? Song { get; set; }

        public string? UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }

        [Required]
        public DateTime AddedAt { get; set; }
    }
}
