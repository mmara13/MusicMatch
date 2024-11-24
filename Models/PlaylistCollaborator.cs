using System.ComponentModel.DataAnnotations;

namespace MusicMatch.Models
{
    public class PlaylistCollaborator
    {
        public int Id { get; set; }

        [Required]
        public int PlaylistId { get; set; }
        public virtual Playlist? Playlist { get; set; }

        [Required]
        public string UserId { get; set; }
        public virtual ApplicationUser? ApplicationUser { get; set; }

        [Required]
        public string PermissionLevel { get; set; } //R W Admin
    }
}
