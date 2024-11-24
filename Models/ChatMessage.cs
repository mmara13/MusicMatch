using System.ComponentModel.DataAnnotations;

namespace MusicMatch.Models
{
    public class ChatMessage
    {
        public int Id { get; set; }

        [Required]
        public int ChatRoomId { get; set; }
        public virtual ChatRoom? ChatRoom { get; set; }

        [Required]
        public string UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }

        [Required]
        public string Content {  get; set; }

        [Required]
        public DateTime Timestamp { get; set; }
    }
}
