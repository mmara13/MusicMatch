using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicMatch.Data;
using MusicMatch.Models;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace MusicMatch.Controllers
{
    public class SongsController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        public SongsController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager
            )
        {
            _context = context;

            _userManager = userManager;

            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index(string searchString)
        {
            var songsQuery = _context.Songs
                    .Include(s => s.Artist)
                    .Include(s => s.Genre)
                    .AsQueryable(); // Ensures compatibility with the Where clause

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                songsQuery = songsQuery.Where(s =>
                    s.Title.Contains(searchString) ||
                    s.Artist.Name.Contains(searchString) ||
                    (s.Genre != null && s.Genre.Name.Contains(searchString))
                );
            }


            return View(await songsQuery.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var song = await _context.Songs
                .Include(s => s.Artist)
                .Include(s => s.Genre)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (song == null) return NotFound();

            return View(song);
        }

        private bool IsValidDuration(string duration)
        {
            var regex = new Regex(@"^([0-9]{2}):([0-9]{2}):([0-9]{2})$");
            return regex.IsMatch(duration);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            PopulateViewData();
            Song song = new Song();
            
            return View(song);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(Song song)
        {
            if (!ModelState.IsValid)
            {
                PopulateViewData();
                return View(song);
            }

            if (!string.IsNullOrEmpty(song.Duration.ToString()))
            {
                if (!IsValidDuration(song.Duration.ToString()))
                {
                    ModelState.AddModelError("Duration", "Invalid format. Please enter in hh:mm:ss format.");
                    return View(song);
                }

                var timeParts = song.Duration.ToString().Split(':');
                if (timeParts.Length == 3)
                {
                    song.Duration = new TimeSpan(int.Parse(timeParts[0]), int.Parse(timeParts[1]), int.Parse(timeParts[2]));
                }
            }

            _context.Add(song);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Song added successfully!";
            return RedirectToAction(nameof(Index));
        }



        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var song = await _context.Songs.FindAsync(id);
            if (song == null) return NotFound();

            PopulateViewData();
            return View(song);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, Song song)
        {
            if (id != song.Id) return NotFound();

            if (!ModelState.IsValid)
            {
                PopulateViewData();
                return View(song);
            }

            if (!string.IsNullOrEmpty(song.Duration.ToString()))
            {
                var timeParts = song.Duration.ToString().Split(':');
                if (timeParts.Length == 3)
                {
                    song.Duration = new TimeSpan(int.Parse(timeParts[0]), int.Parse(timeParts[1]), int.Parse(timeParts[2]));
                }
            }

            _context.Update(song);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Song updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> AddToFavorites(int songId)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var userPreferences = await _context.UserPreferencesForms
                .Include(upf => upf.UserPreferencesSongs)
                .FirstOrDefaultAsync(upf => upf.UserId == userId);

            if (userPreferences == null)
            {
                userPreferences = new UserPreferencesForm
                {
                    UserId = userId,
                };
                _context.UserPreferencesForms.Add(userPreferences);
                await _context.SaveChangesAsync();
            }

            if (userPreferences.UserPreferencesSongs.Any(ups => ups.SongId == songId))
            {
                TempData["ErrorMessage"] = "This song is already in your favorites.";
                return RedirectToAction("Details", "Songs", new { id = songId });
            }

            var userPreferenceSong = new UserPreferencesSong
            {
                SongId = songId,
                UserPreferencesFormId = userPreferences.Id
            };
            _context.UserPreferencesSongs.Add(userPreferenceSong);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Song added to favorites successfully.";
            return RedirectToAction("Details", "Songs", new { id = songId });
        }



        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var song = await _context.Songs
                .Include(s => s.Artist)
                .Include(s => s.Genre)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (song == null) return NotFound();

            return View(song);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var song = await _context.Songs.FindAsync(id);
            if (song == null) return NotFound();

            _context.Songs.Remove(song);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Song deleted successfully!";
            return RedirectToAction(nameof(Index));
        }

        [Authorize]
        public async Task<IActionResult> SelectPlaylistForSong(int songId)
        {
            


            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["ErrorMessage"] = "You must be logged in to add a song to a playlist.";
                return RedirectToAction("Details", new { id = songId });
            }

            var playlists = await _context.Playlists
                .Where(p => p.UserId == user.Id)
                .ToListAsync();

            if (!playlists.Any())
            {
                TempData["ErrorMessage"] = "You don't have any playlists yet. Please create one first.";
                return RedirectToAction("Details", new { id = songId });
            }

            ViewData["SongId"] = songId;
            return View(playlists);
        }

        private void PopulateViewData()
        {
            ViewBag.Artists = _context.Artists.ToList();
            ViewBag.Genres = _context.Genres.ToList();
            ViewBag.Moods = new List<string> { "Happy", "Sad", "Energetic", "Calm", "Angry" };
        }
    }
}
