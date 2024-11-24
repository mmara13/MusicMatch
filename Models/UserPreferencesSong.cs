using System.ComponentModel.DataAnnotations;

namespace MusicMatch.Models
{
    public class UserPreferencesSong
    {
        public int UserPreferencesFormId { get; set; }
        public virtual UserPreferencesForm? UserPreferencesForm { get; set; }
        [Required]  
        public int SongId { get; set; }
        
        public virtual Song? Song { get; set; }
    }
}
