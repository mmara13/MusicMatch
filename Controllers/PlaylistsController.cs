using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicMatch.Data;
using MusicMatch.Models;
using System.Diagnostics;

namespace MusicMatch.Controllers
{
    public class PlaylistsController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        public PlaylistsController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager
            )
        {
            _context = context;

            _userManager = userManager;

            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var playlists = await _context.Playlists
                                          .Where(p => p.UserId == userId)
                                          .Include(p => p.User)
                                          .ToListAsync();
            return View(playlists);
        }

        public async Task<IActionResult> Details(int id)
        {
            var playlist = await _context.Playlists
                                .Include(p => p.Songs)
                .ThenInclude(ps => ps.Song)
            .ThenInclude(s => s.Artist) // Make sure the Artist is included
                 .FirstOrDefaultAsync(p => p.Id == id);


            if (playlist == null)
            {
                return NotFound();
            }

            return View(playlist);
        }

        [HttpGet]
        public IActionResult Create()
        {
            Playlist playlist = new Playlist();

            return View(playlist);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Playlist playlist)
        {
            playlist.UserId = _userManager.GetUserId(User);
            playlist.CreatedDate = DateTime.Now;
            playlist.Visibility ??= "Private";

            if (ModelState.IsValid)
            {
                _context.Playlists.Add(playlist);
                _context.SaveChanges();

                TempData["SuccessMessage"] = "Playlist created successfully!";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return View(playlist);
            }
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var playlist = await _context.Playlists.FindAsync(id);
            if (playlist == null)
            {
                return NotFound();
            }
            return View(playlist);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,IsCollaborative,Visibility")] Playlist playlist)
        {
            if (id != playlist.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(playlist);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PlaylistExists(playlist.Id))
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
            return View(playlist);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var playlist = await _context.Playlists
                .FirstOrDefaultAsync(m => m.Id == id);
            if (playlist == null)
            {
                return NotFound();
            }

            return View(playlist);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var playlist = await _context.Playlists.FindAsync(id);
            if (playlist != null)
            {
                _context.Playlists.Remove(playlist);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddSongToPlaylist(int songId, int playlistId)
        {

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["ErrorMessage"] = "You must be logged in to perform this action.";
                return RedirectToAction("Index", "Home");
            }

            var playlist = await _context.Playlists.FindAsync(playlistId);
            if (playlist == null || playlist.UserId != user.Id)
            {
                TempData["ErrorMessage"] = "Playlist not found or you don't have access to it.";
                return RedirectToAction("Index", "Home");
            }

            var song = await _context.Songs.FindAsync(songId);
            if (song == null)
            {
                TempData["ErrorMessage"] = "Song not found.";
                return RedirectToAction("Index", "Home");
            }

            var existingEntry = await _context.PlaylistSongs
                .FirstOrDefaultAsync(ps => ps.PlaylistId == playlistId && ps.SongId == songId);

            if (existingEntry != null)
            {
                TempData["ErrorMessage"] = "This song is already in the selected playlist.";
                return RedirectToAction("Details", "Songs", new { id = songId });
            }

            var playlistSong = new PlaylistSong
            {
                PlaylistId = playlistId,
                SongId = songId,
                UserId = user.Id,
                AddedAt = DateTime.Now
            };

            _context.PlaylistSongs.Add(playlistSong);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Song '{song.Title}' added to playlist '{playlist.Name}'!";
            return RedirectToAction("Details", "Songs", new { id = songId });
        }


        private bool PlaylistExists(int id)
        {
            return _context.Playlists.Any(e => e.Id == id);
        }
    }
}