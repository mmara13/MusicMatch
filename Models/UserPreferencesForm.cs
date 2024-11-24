using System.ComponentModel.DataAnnotations;

namespace MusicMatch.Models
{
    public class UserPreferencesForm
    {
        public int Id { get; set; }
        [Required]
        public string UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }
        // Navigation properties for many-to-many relationships
        public virtual ICollection<UserPreferencesSong>? UserPreferencesSongs { get; set; } = new List<UserPreferencesSong>();
        public virtual ICollection<UserPreferencesArtist>? UserPreferencesArtists { get; set; } = new List<UserPreferencesArtist>();
    }
}
