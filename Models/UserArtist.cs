using System.ComponentModel.DataAnnotations;

namespace MusicMatch.Models
{
    public class UserArtist
    {
        public int Id { get; set; }
        [Required]
        public int ArtistId { get; set; }
        public virtual Artist? Artist { get; set; }
        [Required]
        public string UserId { get; set; }
        public virtual ApplicationUser? ApplicationUser { get; set; }
    }
}
