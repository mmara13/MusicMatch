using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicMatch.Data;
using MusicMatch.Models;
using System.IO;
using System.Threading.Tasks;

namespace MusicMatch.Controllers
{
    public class ProfilesController : Controller
    {
        private readonly ApplicationDbContext db;

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public ProfilesController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<ApplicationUser> signInManager
            )
        {
            db = context;

            _userManager = userManager;

            _roleManager = roleManager;
            _signInManager = signInManager;
        }

        //// GET: Profiles/Details/userId
        //public async Task<IActionResult> Details(string id)
        //{
        //    if (string.IsNullOrEmpty(id))
        //    {
        //        // If no ID is provided and user is logged in, redirect to their profile
        //        if (User.Identity.IsAuthenticated)
        //        {
        //            var currentUser = await _userManager.GetUserAsync(User);
        //            return RedirectToAction(nameof(Details), new { id = currentUser.Id });
        //        }
        //        return NotFound();
        //    }

        //    var user = await _userManager.Users
        //        .FirstOrDefaultAsync(u => u.Id == id);

        //    if (user == null)
        //    {
        //        return NotFound();
        //    }

        //    ViewBag.IsOwnProfile = false;
        //    if (User.Identity.IsAuthenticated)
        //    {
        //        var currentUser = await _userManager.GetUserAsync(User);
        //        ViewBag.IsOwnProfile = currentUser.Id == user.Id;
        //    }

        //    return View(user);
        //}
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                // If no ID is provided and user is logged in, redirect to their profile
                if (User.Identity.IsAuthenticated)
                {
                    var currentUser = await _userManager.GetUserAsync(User);
                    return RedirectToAction(nameof(Details), new { id = currentUser.Id });
                }
                return NotFound();
            }

            // Get user
            var user = await _userManager.Users
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            // Set ViewBag.IsOwnProfile
            ViewBag.IsOwnProfile = false;
            if (User.Identity.IsAuthenticated)
            {
                var currentUser = await _userManager.GetUserAsync(User);
                ViewBag.IsOwnProfile = currentUser.Id == user.Id;
            }

            // Load preferences data
            var preferences = await db.UserPreferencesForms
                .Include(upf => upf.UserPreferencesSongs)
                    .ThenInclude(ups => ups.Song)
                        .ThenInclude(s => s.Artist)
                .Include(upf => upf.UserPreferencesArtists)
                    .ThenInclude(upa => upa.Artist)
                .FirstOrDefaultAsync(upf => upf.UserId == id);

            ViewBag.UserPreferences = preferences;

            return View(user);
        }


        [Authorize]
        public async Task<IActionResult> Edit()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            // Clear any stale messages
            TempData.Remove("Message");
            TempData.Remove("ErrorMessage");

            return View(user);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ApplicationUser model, IFormFile? photo)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please correct the errors and try again.";
                return View(model);
            }

            try
            {
                // Handle photo upload if a new photo was provided
                if (photo != null && photo.Length > 0)
                {
                    var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "profiles");
                    if (!Directory.Exists(uploadsPath))
                    {
                        Directory.CreateDirectory(uploadsPath);
                    }
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(photo.FileName);
                    var filePath = Path.Combine(uploadsPath, fileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await photo.CopyToAsync(fileStream);
                    }
                    user.PhotoUrl = $"/images/profiles/{fileName}";
                }
                else
                {
                    user.PhotoUrl = model.PhotoUrl;
                }

                // Update user properties
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.PhoneNumber = model.PhoneNumber;

                // Handle email change
                if (user.Email != model.Email)
                {
                    var setEmailResult = await _userManager.SetEmailAsync(user, model.Email);
                    if (!setEmailResult.Succeeded)
                    {
                        TempData["ErrorMessage"] = "Failed to update email.";
                        return View(model);
                    }

                    var setUserNameResult = await _userManager.SetUserNameAsync(user, model.Email);
                    if (!setUserNameResult.Succeeded)
                    {
                        TempData["ErrorMessage"] = "Failed to update username.";
                        return View(model);
                    }
                }

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    if (user.Email != model.Email)
                    {
                        await _signInManager.RefreshSignInAsync(user);
                    }

                    TempData["Message"] = "Profile updated successfully.";
                    // Redirect to Edit instead of Details to show the success message
                    //return RedirectToAction(nameof(Edit));
                    return RedirectToAction("Details", new { id = user.Id });
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    TempData["ErrorMessage"] = "Failed to update profile. Please try again.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while updating the profile.";
            }

            return View(model);
        }


        // Helper method to upload photo (cloud or local)
        private async Task<string> UploadPhotoToCloud(IFormFile photo)
        {
            var photoUrl = "/images/profiles/" + Guid.NewGuid().ToString() + Path.GetExtension(photo.FileName);

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", photoUrl);
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await photo.CopyToAsync(fileStream);
            }

            return photoUrl;
        }

        public async Task<IActionResult> EditPreferences()
        {
            // Get the current user
            var userId = _userManager.GetUserId(User);
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                TempData["Error"] = "User not found.";
                return RedirectToAction("Index");
            }

            // Get user's preferences form with related data
            var userPreferences = await db.UserPreferencesForms
                .Include(upf => upf.UserPreferencesSongs)
                    .ThenInclude(ups => ups.Song)
                .Include(upf => upf.UserPreferencesArtists)
                    .ThenInclude(upa => upa.Artist)
                .Include(upf => upf.User)
                .FirstOrDefaultAsync(upf => upf.UserId == userId);

            if (userPreferences == null)
            {
                userPreferences = new UserPreferencesForm
                {
                    UserId = userId,
                    User = user,
                    UserPreferencesSongs = new List<UserPreferencesSong>(),
                    UserPreferencesArtists = new List<UserPreferencesArtist>()
                };
            }

            // Convert to IEnumerable to enable LINQ methods
            var likedSongs = userPreferences.UserPreferencesSongs
                .Select(ups => ups.Song)
                .ToList()
                .AsEnumerable();
            var likedArtists = userPreferences.UserPreferencesArtists
                .Select(upa => upa.Artist)
                .ToList()
                .AsEnumerable();

            var model = new UserPreferencesForm
            {
                Id = userPreferences.Id,
                UserId = userId,
                User = user,
                UserPreferencesSongs = userPreferences.UserPreferencesSongs,
                UserPreferencesArtists = userPreferences.UserPreferencesArtists
            };

            // Pass as IEnumerable
            ViewBag.LikedSongs = likedSongs;
            ViewBag.LikedArtists = likedArtists;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPreferences(UserPreferencesForm model)
        {
            var userId = _userManager.GetUserId(User);
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                TempData["Error"] = "User not found.";
                return RedirectToAction("Index");
            }

            // Get or create UserPreferencesForm
            var userPreferences = await db.UserPreferencesForms
                .Include(upf => upf.UserPreferencesSongs)
                .Include(upf => upf.UserPreferencesArtists)
                .FirstOrDefaultAsync(upf => upf.UserId == userId);

            if (userPreferences == null)
            {
                userPreferences = new UserPreferencesForm { UserId = userId };
                db.UserPreferencesForms.Add(userPreferences);
                await db.SaveChangesAsync(); // Save to generate Id
            }

            // Clear existing preferences
            db.UserPreferencesSongs.RemoveRange(userPreferences.UserPreferencesSongs);
            db.UserPreferencesArtists.RemoveRange(userPreferences.UserPreferencesArtists);

            // Add new preferences
            if (model.UserPreferencesSongs != null)
            {
                // Take only first 10 songs
                foreach (var songRelation in model.UserPreferencesSongs.Take(10))
                {
                    db.UserPreferencesSongs.Add(new UserPreferencesSong
                    {
                        UserPreferencesFormId = userPreferences.Id,
                        SongId = songRelation.SongId
                    });
                }
            }

            if (model.UserPreferencesArtists != null)
            {
                // Take only first 5 artists
                foreach (var artistRelation in model.UserPreferencesArtists.Take(5))
                {
                    db.UserPreferencesArtists.Add(new UserPreferencesArtist
                    {
                        UserPreferencesFormId = userPreferences.Id,
                        ArtistId = artistRelation.ArtistId
                    });
                }
            }

            await db.SaveChangesAsync();
            TempData["Message"] = "Preferences updated successfully.";
            return RedirectToAction("EditPreferences");
        }


        //search

        // Add these methods to your ProfilesController class

        [HttpGet]
        public async Task<IActionResult> Search(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return View(new List<ApplicationUser>());
            }

            var users = await _userManager.Users
                .Where(u => u.FirstName.Contains(query) ||
                            u.LastName.Contains(query) ||
                            u.Email.Contains(query))
                .Take(20) // Limit results
                .ToListAsync();

            // Add preferences data for each user
            foreach (var user in users)
            {
                var preferences = await db.UserPreferencesForms
                    .Include(upf => upf.UserPreferencesSongs)
                        .ThenInclude(ups => ups.Song)
                    .Include(upf => upf.UserPreferencesArtists)
                        .ThenInclude(upa => upa.Artist)
                    .FirstOrDefaultAsync(upf => upf.UserId == user.Id);

                ViewData[$"Preferences_{user.Id}"] = preferences;
            }

            return View(users);
        }

        [HttpGet]
        public async Task<IActionResult> SearchPartial(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return PartialView("_SearchResults", new List<ApplicationUser>());
            }

            // Convert query to lowercase for case-insensitive search
            query = query.ToLower();

            var users = await _userManager.Users
                .Where(u => u.FirstName.ToLower().Contains(query) ||
                            u.LastName.ToLower().Contains(query) ||
                            u.Email.ToLower().Contains(query))
                .Take(20)
                .ToListAsync();

            foreach (var user in users)
            {
                var preferences = await db.UserPreferencesForms
                    .Include(upf => upf.UserPreferencesSongs)
                        .ThenInclude(ups => ups.Song)
                    .Include(upf => upf.UserPreferencesArtists)
                        .ThenInclude(upa => upa.Artist)
                    .FirstOrDefaultAsync(upf => upf.UserId == user.Id);

                ViewData[$"Preferences_{user.Id}"] = preferences;
            }

            return PartialView("_SearchResults", users);
        }
    }


}
