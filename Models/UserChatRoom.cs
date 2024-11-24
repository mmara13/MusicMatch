using MusicMatch.Models;
using System.ComponentModel.DataAnnotations;

public class UserChatRoom
{
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; }
    public virtual ApplicationUser? User { get; set; }

    [Required]
    public int ChatRoomId { get; set; }
    public virtual ChatRoom? ChatRoom { get; set; }

    public DateTime JoinedAt { get; set; } // Example of additional data for this relationship
}
