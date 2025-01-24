namespace MusicMatch.Models
{
    public class SearchResults
    {
        public IEnumerable<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
        public IEnumerable<Song> Songs { get; set; } = new List<Song>();
        public IEnumerable<Artist> Artists { get; set; } = new List<Artist>();
        public IEnumerable<ChatRoom> ChatRooms { get; set; } = new List<ChatRoom>();
    }
}
