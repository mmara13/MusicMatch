using MusicMatch.Models;
using System.ComponentModel.DataAnnotations;

public class Notification
{
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; } = string.Empty;

    public ApplicationUser? User { get; set; }

    [Required]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Message { get; set; } = string.Empty;

    [Required]
    public string Type { get; set; } = "General";

    public int? ArtistId { get; set; }

    public Artist? Artist { get; set; }

    public int? EventId { get; set; }

    public string? SenderUserId { get; set; }

    public ApplicationUser? SenderUser { get; set; }

    public bool IsRead { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Required]
    public string Status { get; set; } = "Unread";
}