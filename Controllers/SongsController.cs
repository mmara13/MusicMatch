using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicMatch.Data;
using MusicMatch.Models;

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
                    .AsQueryable(); // Ensures compatibility with the Where clause

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                songsQuery = songsQuery.Where(s =>
                    s.Title.Contains(searchString) ||
                    s.Artist.Name.Contains(searchString) ||
                    (s.Genre != null && s.Genre.Contains(searchString))
                );
            }


            return View(await songsQuery.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var song = await _context.Songs
                .Include(s => s.Artist)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (song == null) return NotFound();

            return View(song);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            PopulateViewData();
            return View();
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

            _context.Add(song);
            await _context.SaveChangesAsync();
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

            _context.Update(song);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var song = await _context.Songs
                .Include(s => s.Artist)
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
            return RedirectToAction(nameof(Index));
        }

        private void PopulateViewData()
        {
            ViewBag.Artists = _context.Artists.ToList();
            ViewBag.Moods = new List<string> { "Happy", "Sad", "Energetic", "Calm", "Angry" };
        }
    }
}
