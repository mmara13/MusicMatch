using System.ComponentModel.DataAnnotations;

namespace MusicMatch.Models
{
    public class UserPreferencesArtist
    {
        [Required]
        public int UserPreferencesFormId { get; set; }
        [Required]
        public int ArtistId { get; set; }

        public virtual UserPreferencesForm? UserPreferencesForm { get; set; }
        public virtual Artist? Artist { get; set; }
    }
}
