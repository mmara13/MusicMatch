using System.ComponentModel.DataAnnotations;

namespace MusicMatch.Models
{
    public class UserGenre
    {
        public int Id { get; set; }
        [Required]
        public int GenreId {  get; set; }
        public virtual Genre? Genre { get; set; }
        [Required]
        public string UserId { get; set; }
        public virtual ApplicationUser? ApplicationUser { get; set; }
    }
}
