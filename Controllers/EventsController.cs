using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicMatch.Data;
using MusicMatch.Services;
using MusicMatch.Models;

namespace MusicMatch.Controllers
{
    public class EventsController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly NotificationService _notificationService;

        public EventsController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            NotificationService notificationService
            )
        {
            _context = context;

            _userManager = userManager;

            _roleManager = roleManager;

            _notificationService = notificationService;

        }

        // GET: Events
        public async Task<IActionResult> Index(string? location = null, string? type = null, string? artist = null)
        {
            if (location != null || artist != null)
            {
                return View(FilterEvents(location, type, artist));
            }

            return View(await _context.Events
                .Include(e => e.Artist)
                .Include(e => e.ChatRoom)
                .ToListAsync());
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewBag.Artists = _context.Artists.ToList();
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Name,Description,DateTime,Location,Type,ArtistId")] Event @event)
        {
            if (ModelState.IsValid)
            {
                _context.Add(@event);
                await _context.SaveChangesAsync();
                
                TempData["SuccessMessage"] = "Event added successfully!";

                var chatRoom = new ChatRoom
                {
                    Name = $"Event: {@event.Name}",
                    CreatedAt = DateTime.Now,
                    Type = "Event",
                    RelatedId = @event.Id,
                };

                _context.Add(chatRoom);  // Add the chat room to the database
                await _context.SaveChangesAsync();  // Save changes

                @event.ChatRoom = chatRoom;
                _context.Update(@event);
                    await _context.SaveChangesAsync();

                await _notificationService.NotifyNewEvent(@event.Id);

                return RedirectToAction(nameof(Index));
            }
            return View(@event);
        }


        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Events.FindAsync(id);
            if (@event == null)
            {
                return NotFound();
            }
            ViewBag.Artists = _context.Artists.ToList();
            return View(@event);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,DateTime,Location,Type,ArtistId")] Event @event)
        {
            if (id != @event.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(@event);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Event updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventExists(@event.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(@event);
        }


        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Events
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var @event = await _context.Events.FindAsync(id);
            if (@event != null)
            {
                _context.Events.Remove(@event);
            }
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Event deleted successfully!";
            return RedirectToAction(nameof(Index));
        }

        //-----------------------------------------------

        [NonAction]
        private List<Event> FilterEvents(string? location, string? type = null, string? artist = null)
        {
            var events = _context.Events.AsQueryable();

            if (!string.IsNullOrEmpty(location))
            {
                events = events.Where(e => e.Location.ToLower().Contains(location.ToLower()));
            }

            if (!string.IsNullOrEmpty(type))
            {
                events = events.Where(e => e.Type.ToLower() == type.ToLower());
            }

            if (!string.IsNullOrEmpty(artist))
            {
                events = events.Where(e => e.Artist != null && e.Artist.Name.ToLower().Contains(artist.ToLower()));
            }

            return events
                .Include(e => e.Artist)
                .Include(e => e.Attendees)
                .Include(e => e.ChatRoom)
                .ToList();
        }


        [HttpGet]
        public async Task<IActionResult> GetEvents([FromQuery] string location, [FromQuery] string? type = null)
        {
            if (string.IsNullOrEmpty(location))
            {
                return BadRequest("Location is required.");
            }

            var events = FilterEvents(location, type);

            if (!events.Any())
            {
                return NotFound("No events found for the specified location and type.");
            }

            return Ok(events);
        }
       

        public async Task<IActionResult> Details(int id)
        {
            var @event = await _context.Events
                .Include(e => e.Artist)
                .Include(e => e.Attendees)
                .ThenInclude(a => a.ApplicationUser)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (@event == null)
            {
                return NotFound(); 
            }

            return View(@event);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RSVP(int eventId, string rsvpStatus)
        {
            if (string.IsNullOrEmpty(rsvpStatus))
            {
                return BadRequest("RSVP status is required.");
            }

            var userId = _userManager.GetUserId(User); //Obt Id-ul utilizatorului curent
            if (userId == null)
            {
                return Unauthorized();
            }

            // Verificare daca utilizatorul are deja un RSPV pt evenimentul respectiv
            var existingRSVP = await _context.EventAttendees
                .FirstOrDefaultAsync(ea => ea.EventId == eventId && ea.UserId == userId);

            if (existingRSVP != null)
            {
                
                existingRSVP.RSVP_Status = rsvpStatus;
                _context.Update(existingRSVP);
            }
            else
            {
                // Creare RSVP nou
                var newRSVP = new EventAttendee
                {
                    EventId = eventId,
                    UserId = userId,
                    RSVP_Status = rsvpStatus
                };
                _context.Add(newRSVP);
            }

            await _context.SaveChangesAsync(); //Salvare modificari

            return RedirectToAction("Details", new { id = eventId }); // redirectionare pagina Details a evenimentului
        }


        //---------------
        private bool EventExists(int id)
        {
            return _context.Events.Any(e => e.Id == id);
        }
    }
}