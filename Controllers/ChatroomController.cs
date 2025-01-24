using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicMatch.Data;

namespace MusicMatch.Controllers
{
    public class ChatroomController : Controller
    {
        private readonly ApplicationDbContext db;

        public ChatroomController(
            ApplicationDbContext context
            )
        {
            db = context;


        }

        // GET: ChatRooms
        public async Task<IActionResult> Index()
        {
            var chatRooms = await db.ChatRooms.ToListAsync();
            return View(chatRooms);
        }


        // GET: ChatRoom/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var chatRoom = await db.ChatRooms
                .Include(c => c.Messages)
                .ThenInclude(m => m.User)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (chatRoom == null)
            {
                return NotFound();
            }

            return View(chatRoom);
        }



        [HttpGet]
        public IActionResult EnterRoom(int id)
        {
            var chatRoom = db.ChatRooms
                .Include(c => c.Messages)
                .FirstOrDefault(c => c.Id == id);

            if (chatRoom == null)
            {
                return NotFound();
            }

            return View(chatRoom);
        }
    }


}

