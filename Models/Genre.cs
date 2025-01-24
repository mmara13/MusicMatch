using System.ComponentModel.DataAnnotations;

namespace MusicMatch.Models
{
    public class Genre
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        public string? Description { get; set; }

        public virtual ICollection<UserGenre>? UserGenres { get; set; }

        public virtual ICollection<Song>? Songs { get; set; }
    }
}
