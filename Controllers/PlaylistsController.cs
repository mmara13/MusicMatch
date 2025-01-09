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

        // GET: Playlists
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var playlists = await _context.Playlists
                                          .Where(p => p.UserId == userId)
                                          .Include(p => p.User)
                                          .ToListAsync();
            return View(playlists);
        }

        // GET: Playlists/Create
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


        // GET: Playlists/Edit/5
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

        // POST: Playlists/Edit/5
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

        // GET: Playlists/Delete/5
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

        // POST: Playlists/Delete/5
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

        private bool PlaylistExists(int id)
        {
            return _context.Playlists.Any(e => e.Id == id);
        }
    }
}