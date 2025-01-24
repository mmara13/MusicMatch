using Microsoft.AspNetCore.SignalR;
using MusicMatch.Models;

namespace MusicMatch.Hubs
{
    public class NotificationHub : Hub
    {
        public async Task SendNotification(string userId, Notification notification)
        {
            await Clients.User(userId).SendAsync("ReceiveNotification", notification);
        }
    }
}