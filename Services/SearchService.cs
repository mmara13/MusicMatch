using Microsoft.EntityFrameworkCore;
using MusicMatch.Data;
using MusicMatch.Models;

public class SearchService
{
    private readonly ApplicationDbContext _context;

    public SearchService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SearchResults> SearchAsync(string query, string[] filterTypes = null, string genre = null)
    {
        query = query?.Trim().ToLower();
        var results = new SearchResults();

        if (!string.IsNullOrEmpty(query))
        {
            if (filterTypes == null || filterTypes.Contains("users"))
            {
                results.Users = await _context.Users
                    .Where(u => u.FirstName.ToLower().Contains(query) ||
                               u.LastName.ToLower().Contains(query) ||
                               u.Email.ToLower().Contains(query))
                    .Take(20)
                    .ToListAsync();
            }

            if (filterTypes == null || filterTypes.Contains("songs"))
            {
                results.Songs = await _context.Songs
                    .Include(s => s.Artist)
                    .Include(s => s.Genre)
                    .Where(s => s.Title.ToLower().Contains(query) ||
                               (s.Artist != null && s.Artist.Name.ToLower().Contains(query)) ||
                               (s.Genre != null && s.Genre.Name.ToLower().Contains(query)) &&
                               (genre == null || s.Genre.Name.ToLower() == genre.ToLower()))
                    .Take(20)
                    .ToListAsync();
            }

            if (filterTypes == null || filterTypes.Contains("artists"))
            {
                results.Artists = await _context.Artists
                    .Where(a => a.Name.ToLower().Contains(query))
                    .Take(20)
                    .ToListAsync();
            }

            if (filterTypes == null || filterTypes.Contains("groups"))
            {
                var chatRoomQuery = _context.ChatRooms.AsQueryable();

                if (!string.IsNullOrEmpty(query))
                {
                    chatRoomQuery = chatRoomQuery.Where(cr => cr.Name.ToLower().Contains(query));
                }

                if (!string.IsNullOrEmpty(genre))
                {
                    chatRoomQuery = chatRoomQuery.Where(cr =>
                        cr.Type == "Genre" && cr.RelatedId.ToString() == genre);
                }

                results.ChatRooms = await chatRoomQuery
                    .Take(20)
                    .ToListAsync();
            }
        }

        return results;
    }
}

