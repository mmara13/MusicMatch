using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicMatch.Data;
using MusicMatch.Services;
using MusicMatch.Models;
using System.Security.Claims;

namespace MusicMatch.Controllers
{
    public class ArtistsController : Controller
    {
        private readonly ApplicationDbContext db;

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly NotificationService _notificationService;


        public ArtistsController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            NotificationService notificationService
            )
        {
            db = context;

            _userManager = userManager;

            _roleManager = roleManager;
            _notificationService = notificationService;
        }

        public async Task<IActionResult> Index(string searchString)
        {
            // Fetch notifications for the current user
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var notifications = await db.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .OrderByDescending(n => n.CreatedAt)
                .Take(5)
                .ToListAsync();

            // Set notifications in ViewBag
            ViewBag.Notifications = notifications;

            // Your existing artist query
            var artists = db.Artists.Include(a => a.Songs).AsQueryable();
            if (!string.IsNullOrEmpty(searchString))
            {
                artists = artists.Where(a =>
                    (a.Name ?? "").Contains(searchString) ||
                    (a.Bio ?? "").Contains(searchString));
            }
            return View(await artists.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var artist = await db.Artists
                .Include(a => a.Songs)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (artist == null) return NotFound();

            return View(artist);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(Artist artist)
        {
            if (!ModelState.IsValid) return View(artist);

            db.Artists.Add(artist);
            await db.SaveChangesAsync();
            await _notificationService.NotifyNewArtist(artist.Id);
            TempData["SuccessMessage"] = "Artist added successfully!";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var artist = await db.Artists.FindAsync(id);
            if (artist == null) return NotFound();

            return View(artist);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, Artist artist)
        {
            if (id != artist.Id) return NotFound();

            if (!ModelState.IsValid) return View(artist);

            db.Update(artist);
            await db.SaveChangesAsync();
            TempData["SuccessMessage"] = "Artist updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> AddToFavorites(int artistId)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var userPreferences = await db.UserPreferencesForms
                .Include(upf => upf.UserPreferencesArtists)
                .FirstOrDefaultAsync(upf => upf.UserId == userId);

            if (userPreferences == null)
            {
                userPreferences = new UserPreferencesForm
                {
                    UserId = userId,
                };
                db.UserPreferencesForms.Add(userPreferences);
                await db.SaveChangesAsync();
            }

            if (userPreferences.UserPreferencesArtists.Any(upa => upa.ArtistId == artistId))
            {
                TempData["ErrorMessage"] = "This artist is already in your favorites.";
                return RedirectToAction("Details", "Artists", new { id = artistId });
            }

            var userPreferenceArtist = new UserPreferencesArtist
            {
                ArtistId = artistId,
                UserPreferencesFormId = userPreferences.Id
            };
            db.UserPreferencesArtists.Add(userPreferenceArtist);
            await db.SaveChangesAsync();

            TempData["SuccessMessage"] = "Artist added to favorites successfully.";
            return RedirectToAction("Details", "Artists", new { id = artistId });
        }


        public async Task<IActionResult> Delete(int id)
        {
            var artist = await db.Artists
                .Include(a => a.Songs)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (artist == null)
            {
                return NotFound();
            }

            // First, delete related notifications
            var relatedNotifications = await db.Notifications
                .Where(n => n.ArtistId == id)
                .ToListAsync();

            db.Notifications.RemoveRange(relatedNotifications);

            // Then remove the artist and its songs
            db.Songs.RemoveRange(artist.Songs);
            db.Artists.Remove(artist);

            await db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var artist = await db.Artists.FindAsync(id);
            if (artist == null) return NotFound();

            db.Artists.Remove(artist);
            await db.SaveChangesAsync();
            TempData["SuccessMessage"] = "Artist deleted successfully!";
            return RedirectToAction(nameof(Index));
        }
    }
}
