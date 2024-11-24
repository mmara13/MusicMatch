using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicMatch.Data;
using MusicMatch.Models;

namespace MusicMatch.Controllers
{
    public class ArtistsController : Controller
    {
        private readonly ApplicationDbContext db;

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        public ArtistsController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager
            )
        {
            db = context;

            _userManager = userManager;

            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index(string searchString)
        {
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

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var artist = await db.Artists.FirstOrDefaultAsync(a => a.Id == id);
            if (artist == null) return NotFound();

            return View(artist);
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var artist = await db.Artists.FindAsync(id);
            if (artist == null) return NotFound();

            db.Artists.Remove(artist);
            await db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
