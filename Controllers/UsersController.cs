using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicMatch.Data;
using MusicMatch.Models;
using Microsoft.Extensions.Logging;

namespace MusicMatch.Controllers
{
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext db;

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        public UsersController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager
            )
        {
            db = context;

            _userManager = userManager;

            _roleManager = roleManager;
        }

        [Authorize(Roles = "Admin")] //vom folosi controllerul asta doar pt admin related actions
        public async Task<IActionResult> Index()
        {
            // Fetch the users and their related data
            var users = await _userManager.Users
                .Include(u => u.FavoriteGenres).ThenInclude(fg => fg.Genre)
                .Include(u => u.FavoriteArtists).ThenInclude(fa => fa.Artist)
                .Include(u => u.RecentActivities)
                .ToListAsync();

            ViewBag.Message = TempData["Message"];
            return View(users); // Use a view that expects List<ApplicationUser>
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            // Fetch the specific user and their related data
            var user = await _userManager.Users
                .Include(u => u.FavoriteGenres).ThenInclude(fg => fg.Genre)
                .Include(u => u.FavoriteArtists).ThenInclude(fa => fa.Artist)
                .Include(u => u.RecentActivities)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null) return NotFound();

            return View(user); // Use a view that expects ApplicationUser
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string id)  // Changed from DeleteUser to Delete to match the route
        {
            try
            {
                // Find user with all related entities
                var user = await db.Users
                    .Include(u => u.FavoriteGenres)
                    .Include(u => u.FavoriteArtists)
                    .Include(u => u.RecentActivities)
                    .Include(u => u.EventAttendances)
                    .Include(u => u.CreatedPlaylists)
                    .Include(u => u.CollaborativePlaylists)
                    .Include(u => u.ChatRooms)
                    .Include(u => u.ChatMessages)
                    .Include(u => u.Moods)
                    .Include(u => u.Matches)
                    .Include(u => u.PlaylistSongs)
                    .Include(u => u.Songs)
                    .Include(u => u.UserChatRooms)
                    .FirstOrDefaultAsync(u => u.Id == id);

                if (user == null)
                {
                    TempData["Error"] = "User not found.";
                    return RedirectToAction("Index");
                }

                // Remove related entities
                if (user.FavoriteGenres?.Any() == true)
                {
                    db.UserGenres.RemoveRange(user.FavoriteGenres);
                }

                if (user.FavoriteArtists?.Any() == true)
                {
                    db.UserArtists.RemoveRange(user.FavoriteArtists);
                }

                if (user.RecentActivities?.Any() == true)
                {
                    db.ActivityLogs.RemoveRange(user.RecentActivities);
                }

                if (user.EventAttendances?.Any() == true)
                {
                    db.EventAttendees.RemoveRange(user.EventAttendances);
                }

                if (user.CreatedPlaylists?.Any() == true)
                {
                    foreach (var playlist in user.CreatedPlaylists)
                    {
                        if (playlist.Songs != null)
                        {
                            db.PlaylistSongs.RemoveRange(playlist.Songs);
                        }
                        db.Playlists.Remove(playlist);
                    }
                }

                if (user.CollaborativePlaylists?.Any() == true)
                {
                    db.PlaylistCollaborators.RemoveRange(user.CollaborativePlaylists);
                }

                if (user.ChatMessages?.Any() == true)
                {
                    db.ChatMessages.RemoveRange(user.ChatMessages);
                }

                if (user.UserChatRooms?.Any() == true)
                {
                    db.UserChatRooms.RemoveRange(user.UserChatRooms);
                }

                if (user.Moods?.Any() == true)
                {
                    db.UserMoods.RemoveRange(user.Moods);
                }

                if (user.Matches?.Any() == true)
                {
                    db.UserMatches.RemoveRange(user.Matches);
                }

                if (user.PlaylistSongs?.Any() == true)
                {
                    db.PlaylistSongs.RemoveRange(user.PlaylistSongs);
                }

                if (user.Songs?.Any() == true)
                {
                    db.UserSongs.RemoveRange(user.Songs);
                }

                // Remove the user using UserManager instead of direct db context
                await _userManager.DeleteAsync(user);
                await db.SaveChangesAsync();

                TempData["Message"] = "User successfully deleted.";
                TempData.Keep("Message"); // Ensure the message is retained after the redirect.
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                TempData["Error"] = "An error occurred while deleting the user.";
                return RedirectToAction("Index");
            }
        }



        //public async Task<IActionResult> Edit(string id)
        //{
        //    if (string.IsNullOrEmpty(id))
        //    {
        //        return NotFound();
        //    }

        //    var user = await _userManager.Users
        //        .Include(u => u.FavoriteGenres)
        //        .Include(u => u.FavoriteArtists)
        //        .FirstOrDefaultAsync(u => u.Id == id);

        //    if (user == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(user);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(string id, [Bind("Id,UserName,Email,PhoneNumber")] ApplicationUser updatedUser)
        //{
        //    if (id != updatedUser.Id)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            var user = await _userManager.FindByIdAsync(id);
        //            if (user == null)
        //            {
        //                return NotFound();
        //            }

        //            // Update user properties
        //            user.UserName = updatedUser.UserName;
        //            user.Email = updatedUser.Email;
        //            user.PhoneNumber = updatedUser.PhoneNumber;

        //            var result = await _userManager.UpdateAsync(user);

        //            if (result.Succeeded)
        //            {
        //                TempData["SuccessMessage"] = "User updated successfully.";
        //                return RedirectToAction(nameof(Index));
        //            }

        //            foreach (var error in result.Errors)
        //            {
        //                ModelState.AddModelError(string.Empty, error.Description);
        //            }
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!UserExists(updatedUser.Id))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //    }

        //    return View(updatedUser);
        //}

        //private bool UserExists(string id)
        //{
        //    return _userManager.Users.Any(e => e.Id == id);
        //}



        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ReportUser(string reportedUserId, string reason)
        {
            var reportingUserId = _userManager.GetUserId(User); // Get the ID of the current user

            if (string.IsNullOrEmpty(reason))
            {
                TempData["ErrorMessage"] = "Please provide a reason for reporting the user.";
                return RedirectToAction("Details", new { id = reportedUserId });
            }

            // Create a new report entry
            var userReport = new UserReport
            {
                ReportedUserId = reportedUserId,
                ReportedById = reportingUserId,
                Reason = reason,
                ReportedAt = DateTime.Now
            };

            // Add the report to the database
            db.UserReports.Add(userReport);
            await db.SaveChangesAsync();

            TempData["SuccessMessage"] = "User reported successfully!";
            return RedirectToAction("Details", new { id = reportedUserId });
        }

    }
}