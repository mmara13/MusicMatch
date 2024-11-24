using System.ComponentModel.DataAnnotations;

namespace MusicMatch.Models
{
    public class Artist
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; }

        public string? Bio { get; set; }
        public string? PhotoUrl { get; set; }

        public virtual ICollection<Song>? Songs { get; set; }

        public virtual ICollection<UserArtist>? UserArtists { get; set; }
        public virtual ICollection<UserPreferencesArtist>? UserPreferencesArtists { get; set; }
    }
}
