using System.ComponentModel.DataAnnotations;

namespace MusicMatch.Models
{
    public class UserMatch
    {
        public int Id { get; set; }

        [Required]
        public string User1Id { get; set; }
        public virtual ApplicationUser? User1 { get; set; }

        [Required]
        public string User2Id { get; set; }
        public virtual ApplicationUser? User2 { get; set; }

        [Required]
        [Range(0,100)]
        public int MatchScore { get; set; }

        [Required]
        public DateTime MatchDate {  get; set; }

        [Required]
        public string Status { get; set; } //Pending, Accepted, Rejected
    }
}
