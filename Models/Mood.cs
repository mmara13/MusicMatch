

using System.ComponentModel.DataAnnotations;

namespace MusicMatch.Models
{
    public class Mood
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name {  get; set; }

        public string? Description { get; set; }

        public virtual ICollection<UserMood> UserMoods { get; set; }
    }
}
