using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace MusicMatch.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        public string? PhotoUrl { get; set; }

        //preferinte useri
        public virtual ICollection<UserGenre>? FavoriteGenres { get; set; }
        public virtual ICollection<UserArtist>? FavoriteArtists { get; set; }
        public virtual ICollection<ActivityLog>? RecentActivities { get; set; }
        public virtual ICollection<EventAttendee>? EventAttendances {  get; set; }
        public virtual ICollection<Playlist>? CreatedPlaylists { get; set; }
        public virtual ICollection<PlaylistCollaborator>? CollaborativePlaylists { get; set; }
        public virtual ICollection<ChatRoom>? ChatRooms { get; set; }
        public virtual ICollection<ChatMessage>? ChatMessages { get; set; }
        public virtual ICollection<UserMood>? Moods { get; set; }
        public virtual ICollection<UserMatch>? Matches { get; set; }
        public virtual ICollection<PlaylistSong>? PlaylistSongs { get; set; }
        public virtual ICollection<UserSong>? Songs { get; set; }
        public virtual ICollection<UserChatRoom>? UserChatRooms { get; set; }
        public virtual ICollection<UserPreferencesForm>? Preferences { get; set; }
    }

}
