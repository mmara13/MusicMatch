using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicMatch.Data;
using MusicMatch.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MusicMatch.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ModerationToolsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ModerationToolsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: ModerationTools/Index
        public async Task<IActionResult> Index()
        {
            var reports = await _context.UserReports
                .Include(r => r.ReportedUser)
                .Include(r => r.ReportedBy)
                .ToListAsync();

            return View(reports);
        }

        // GET: ModerationTools/Details/{id}
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var report = await _context.UserReports
                .Include(r => r.ReportedUser)
                .Include(r => r.ReportedBy)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (report == null)
            {
                return NotFound();
            }

            return View(report);
        }

        // POST: ModerationTools/Resolve/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Resolve(int id)
        {
            var report = await _context.UserReports.FindAsync(id);

            if (report == null)
            {
                return NotFound();
            }

            // Marcare raport ca rezolvat
            report.IsResolved = true;
            _context.Update(report);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "The report has been resolved.";
            return RedirectToAction(nameof(Index));
        }

        // POST: ModerationTools/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var report = await _context.UserReports.FindAsync(id);

            if (report == null)
            {
                return NotFound();
            }

            _context.UserReports.Remove(report);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "The report has been deleted.";
            return RedirectToAction(nameof(Index));
        }

        // GET: ModerationTools/ClearResolvedReports
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ClearResolvedReports()
        {
            var resolvedReports = _context.UserReports.Where(r => r.IsResolved);
            _context.UserReports.RemoveRange(resolvedReports);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "All resolved reports have been cleared.";
            return RedirectToAction(nameof(Index));
        }


    }
}
