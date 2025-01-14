using Microsoft.AspNetCore.SignalR;
using MusicMatch.Data;
using MusicMatch.Hubs;
using MusicMatch.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public class NotificationService
{
    private readonly ApplicationDbContext _context;
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly UserManager<ApplicationUser> _userManager;

    public NotificationService(
        ApplicationDbContext context,
        IHubContext<NotificationHub> hubContext,
        UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _hubContext = hubContext;
        _userManager = userManager;
    }

    public async Task NotifyNewArtist(int artistId)
    {
        var artist = await _context.Artists.FindAsync(artistId);
        if (artist == null) return;

        var allUsers = await _userManager.GetUsersInRoleAsync("User");
        var editorUsers = await _userManager.GetUsersInRoleAsync("Editor");
        var usersToNotify = allUsers.Concat(editorUsers).Select(u => u.Id).Distinct();

        foreach (var userId in usersToNotify)
        {
            try
            {
                var notification = new Notification
                {
                    UserId = userId,
                    Title = "New Artist Alert!",
                    Message = $"Welcome {artist.Name} to our platform! Check out their profile!",
                    Type = "NewArtist",
                    CreatedAt = DateTime.UtcNow,
                    IsRead = false,
                    Status = "Unread",
                    ArtistId = artistId
                };

                _context.Notifications.Add(notification);
                await _context.SaveChangesAsync();

                // Send real-time notification
                await _hubContext.Clients.User(userId).SendAsync("ReceiveNotification", new
                {
                    notification.Id,
                    notification.Title,
                    notification.Message,
                    notification.CreatedAt,
                    notification.IsRead,
                    notification.Status
                });
            }
            catch (Exception ex)
            {
                // Log the error but continue with other users
                Console.WriteLine($"Error creating notification for user {userId}: {ex.Message}");
            }
        }
    }

    public async Task NotifyNewSong(int songId)
    {
        var song = await _context.Songs
            .Include(s => s.Artist)
            .FirstOrDefaultAsync(s => s.Id == songId);

        if (song?.Artist == null) return;

        var allUsers = await _userManager.GetUsersInRoleAsync("User");
        var editorUsers = await _userManager.GetUsersInRoleAsync("Editor");
        var usersToNotify = allUsers.Concat(editorUsers).Select(u => u.Id).Distinct();

        foreach (var userId in usersToNotify)
        {
            try
            {
                var notification = new Notification
                {
                    UserId = userId,
                    Title = "New Song Released!",
                    Message = $"{song.Artist.Name} just released '{song.Title}'! Check it out!",
                    Type = "NewSong",
                    CreatedAt = DateTime.UtcNow,
                    IsRead = false,
                    Status = "Unread",
                    ArtistId = song.ArtistId
                };

                _context.Notifications.Add(notification);
                await _context.SaveChangesAsync();

                await _hubContext.Clients.User(userId).SendAsync("ReceiveNotification", new
                {
                    notification.Id,
                    notification.Title,
                    notification.Message,
                    notification.CreatedAt,
                    notification.IsRead,
                    notification.Status
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating notification for user {userId}: {ex.Message}");
            }
        }
    }

    public async Task NotifyNewEvent(int eventId)
    {
        var ev = await _context.Events
            .Include(e => e.Artist)
            .FirstOrDefaultAsync(e => e.Id == eventId);

        if (ev == null) return;

        var allUsers = await _userManager.GetUsersInRoleAsync("User");
        var editorUsers = await _userManager.GetUsersInRoleAsync("Editor");
        var usersToNotify = allUsers.Concat(editorUsers).Select(u => u.Id).Distinct();

        foreach (var userId in usersToNotify)
        {
            try
            {
                var notification = new Notification
                {
                    UserId = userId,
                    Title = "New Event Alert! 🎫",
                    Message = $"Don't miss out! {ev.Name} is happening on {ev.DateTime.ToShortDateString()} at {ev.Location}! Type: {ev.Type}",
                    Type = "NewEvent",
                    CreatedAt = DateTime.UtcNow,
                    IsRead = false,
                    Status = "Unread",
                    EventId = eventId
                };

                _context.Notifications.Add(notification);
                await _context.SaveChangesAsync();

                await _hubContext.Clients.User(userId).SendAsync("ReceiveNotification", new
                {
                    notification.Id,
                    notification.Title,
                    notification.Message,
                    notification.CreatedAt,
                    notification.IsRead,
                    notification.Status
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating notification for user {userId}: {ex.Message}");
            }
        }
    }
}