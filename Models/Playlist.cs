using System.ComponentModel.DataAnnotations;

namespace MusicMatch.Models
{
    public class Playlist
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; }

        public string? Description { get; set; }

        public string? Mood {  get; set; }
        public string? Genre { get; set; }

        [Required]
        public DateTime? CreatedDate { get; set; }

        [Required]
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        [Required]
        public bool IsCollaborative { get; set; }

        [Required]
        public string Visibility { get; set; } //Public Private Friends

        public virtual ICollection<PlaylistCollaborator>? Collaborators { get; set; }
        public virtual ICollection<PlaylistSong>? Songs { get; set; }
    }
}
