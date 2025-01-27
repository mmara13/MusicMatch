using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicMatch.Data;
using MusicMatch.Models;
using System.Diagnostics;

namespace MusicMatch.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            // Playlists based on mood/genre recommendations
            var moods = new List<string> { "Happy", "Sad", "Energetic", "Calm", "Angry" };
            var genres = await _context.Genres.ToListAsync();

            var numberOfPlaylistsGenerated = 5;

            ViewBag.genrePlaylists = new List<Playlist>();
            ViewBag.moodPlaylists = new List<Playlist>();

            for (var i = 0; i < numberOfPlaylistsGenerated; i++)
            {
                var randomGenre = genres[new Random().Next(genres.Count)];
                var randomMood = moods[new Random().Next(moods.Count)];

                var genrePlaylistName = $"{randomGenre.Name} Playlist";
                var moodPlaylistName = $"{randomMood} Playlist";

                var genrePlaylist = await _context.Playlists
                    .Include(p => p.Songs)
                    .FirstOrDefaultAsync(p => p.Name == genrePlaylistName);

                if (genrePlaylist == null)
                {
                    genrePlaylist = new Playlist()
                    {
                        Name = genrePlaylistName,
                        Description = $"A playlist for {randomGenre.Name} songs",
                        Genre = randomGenre.Name,
                        Visibility = "Public",
                        CreatedDate = DateTime.Now,
                        IsCollaborative = false,
                        Songs = (await _context.Songs
                            .Where(s => s.GenreId == randomGenre.Id)
                            .OrderBy(s => Guid.NewGuid())
                            .Take(5)
                            .ToListAsync()).Select(s => new PlaylistSong { Song = s, AddedAt = DateTime.Now}).ToList()
                    };

                    _context.Add(genrePlaylist);
                }

                var moodPlaylist = await _context.Playlists
                    .Include(p => p.Songs)
                    .FirstOrDefaultAsync(p => p.Name == moodPlaylistName);

                if (moodPlaylist == null)
                {
                    moodPlaylist = new Playlist()
                    {
                        Name = moodPlaylistName,
                        Description = $"A playlist for {randomMood} songs",
                        Visibility = "Public",
                        CreatedDate = DateTime.Now,
                        IsCollaborative = false,
                        Songs = (await _context.Songs
                            .Where(s => s.Mood == randomMood)
                            .OrderBy(s => Guid.NewGuid())
                            .Take(5)
                            .ToListAsync()).Select(s => new PlaylistSong { Song = s, AddedAt= DateTime.Now }).ToList()
                    };

                    _context.Add(moodPlaylist);
                }

                ViewBag.genrePlaylists.Add(genrePlaylist);
                ViewBag.moodPlaylists.Add(moodPlaylist);
            }

            //Music Discovery Recommendations
            var discoverySongs = await _context.Songs
                .OrderBy(s => Guid.NewGuid())
                .Take(10)
                .ToListAsync();

            ViewBag.discoverySongs = discoverySongs;

            await _context.SaveChangesAsync();

            return View();
        }



        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            var exceptionHandlerPathFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            var errorMessage = exceptionHandlerPathFeature?.Error?.Message ?? "An error occurred.";

            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                Message = errorMessage
            });
        }
    }
}
