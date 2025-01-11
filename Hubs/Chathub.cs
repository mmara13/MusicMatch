using Microsoft.AspNetCore.SignalR;

namespace MusicMatch.Hubs
{
    public class ChatHub : Hub
    {

        // Join a group (chat room)
        public async Task JoinChatRoom(int chatroomId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, chatroomId.ToString());
        }


        // Send a message to all clients in the same chat room
        public async Task SendMessageToChatRoom(int chatroomId, string userId, string content)
        {
            // You can send the message to all users in the specified chat room
            await Clients.Group(chatroomId.ToString()).SendAsync("ReceiveMessage", userId, content);
        }

      
        // Leave a group (chat room)
        public async Task LeaveChatRoom(int chatroomId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatroomId.ToString());
        }
    }
}
