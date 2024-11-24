using System.ComponentModel.DataAnnotations;

namespace MusicMatch.Models
{
    public class ChatRoom
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; }

        [Required] 
        public string Type {  get; set; } //Event genre artist

        [Required]
        public int RelatedId { get; set; } //EventId/GenreId sau ArtistId

        [Required]
        public DateTime CreatedAt { get; set; }

        public virtual ICollection<ChatMessage>? Messages { get; set; }
        public virtual ICollection<ApplicationUser>? Participants { get; set; }

        public virtual ICollection<UserChatRoom>? UserChatRooms { get; set; }
    }
}
