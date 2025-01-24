using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using MusicMatch.Data;
using MusicMatch.Hubs; // Ensure your SignalR hub namespace is included
using MusicMatch.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MusicMatch.Controllers
{
    public class ChatMessageController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHubContext<ChatHub> _hubContext;

        // Constructor
        public ChatMessageController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IHubContext<ChatHub> hubContext)
        {
            _db = context;
            _userManager = userManager;
            _hubContext = hubContext;
        }
        public async Task<string> GetUsernameById(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId); 
            return user?.UserName ?? "Unknown";
        }


        [HttpPost]
        public async Task<IActionResult> SendMessage(int chatroomId, string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                return BadRequest("Message content cannot be empty.");
            }

            // Find the chat room
            var chatRoom = await _db.ChatRooms
                .Include(c => c.Messages) // Include messages
                .FirstOrDefaultAsync(c => c.Id == chatroomId);

            if (chatRoom == null)
            {
                return NotFound("Chatroom not found.");
            }

            // Get the current user
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized("User not authenticated.");
            }

            // Create a new message
            var message = new ChatMessage
            {
                Content = content,
                Timestamp = DateTime.Now,
                UserId = user.Id // Use the authenticated user's ID
                
            };
            var username = GetUsernameById(message.UserId);

            // Add the message to the chatroom
            chatRoom.Messages.Add(message);
            await _db.SaveChangesAsync();

          
            await _hubContext.Clients.Group(chatroomId.ToString())
                .SendAsync("ReceiveMessage", user.UserName, content);

            return Ok();
        }
    }
}
