using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicMatch.Data;
using MusicMatch.Models;

namespace MusicMatch.Controllers
{
    public class GenresController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        public GenresController(
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
            var genresQuery = _context.Genres
                    .Include(g => g.Songs)
                    .ThenInclude(s => s.Artist)
                    .AsQueryable(); // Ensures compatibility with the Where clause

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                genresQuery = genresQuery.Where(g =>
                    g.Name.Contains(searchString) ||
                    g.Songs.Any(s => s.Title.Contains(searchString) ||
                                     (s.Artist != null && s.Artist.Name.Contains(searchString)))
                );
            }

            return View("Index", await genresQuery.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var genre = await _context.Genres
                .Include(g => g.Songs)
                .ThenInclude(s => s.Artist)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (genre == null) return NotFound();

            return View(genre);
        }


        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(Genre genre)
        {
            if (!ModelState.IsValid)
            {
                return View(genre);
            }

            _context.Add(genre);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Genre added successfully!";
            return RedirectToAction(nameof(Index));
        }



        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var genre = await _context.Genres.FindAsync(id);
            if (genre == null) return NotFound();

            return View(genre);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, Genre genre)
        {
            if (id != genre.Id) return NotFound();

            if (!ModelState.IsValid)
            {
                return View(genre);
            }

            _context.Update(genre);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Genre updated successfully!";
            return RedirectToAction(nameof(Index));
        }


        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var genre = await _context.Genres
                .Include(g => g.Songs)
                .ThenInclude(s => s.Artist)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (genre == null) return NotFound();

            return View(genre);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var genre = await _context.Genres.FindAsync(id);
            if (genre == null) return NotFound();

            _context.Genres.Remove(genre);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Genre deleted successfully!";
            return RedirectToAction(nameof(Index));
        }

    }
}
