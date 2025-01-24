using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MusicMatch.Models
{
    public class Song
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [Required]
        public int? ArtistId { get; set; }
        public virtual Artist? Artist { get; set; }

        public int? GenreId { get; set; }
        public virtual Genre? Genre { get; set; }
        public string? Mood { get; set; }

        [Required]
        public TimeSpan Duration { get; set; }

        public DateTime? ReleaseDate { get; set; }

        public virtual ICollection<PlaylistSong>? Playlists { get; set; }
        public virtual ICollection<UserPreferencesSong>? UserPreferencesSongs { get; set; }
    }
}
