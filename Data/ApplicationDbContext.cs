using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MusicMatch.Models;

namespace MusicMatch.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser> 
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<ActivityLog> ActivityLogs { get; set; }
        public DbSet<UserArtist> UserArtists { get; set; }
        public DbSet<UserGenre> UserGenres { get; set; }
        public DbSet<Artist> Artists { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<EventAttendee> EventAttendees { get; set; }
        public DbSet<Playlist> Playlists { get; set; }
        public DbSet<PlaylistCollaborator> PlaylistCollaborators { get; set; }
        public DbSet<Song> Songs { get; set; }
        public DbSet<PlaylistSong> PlaylistSongs { get; set; }
        public DbSet<ChatRoom> ChatRooms { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }
        public DbSet<Mood> Moods { get; set; }
        public DbSet<UserMood> UserMoods { get; set; }
        public DbSet<UserMatch> UserMatches { get; set; }
        public DbSet<UserSong> UserSongs { get; set; }
        public DbSet<UserChatRoom> UserChatRooms { get; set; }
        public DbSet<UserPreferencesForm> UserPreferencesForms { get; set; }
        public DbSet<UserPreferencesArtist> UserPreferencesArtists { get; set; }
        public DbSet<UserPreferencesSong> UserPreferencesSongs { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);

            //definire primary key compus pt UserGenre
            modelBuilder.Entity<UserGenre>()
                .HasKey(ab => new {
                    ab.Id,
                    ab.GenreId,
                    ab.UserId
                });

         
            modelBuilder.Entity<UserGenre>()
                .HasOne(ug => ug.ApplicationUser)
                .WithMany(u => u.FavoriteGenres)
                .HasForeignKey(ug => ug.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserArtist>()
                .HasOne(ua => ua.ApplicationUser)
                .WithMany(u => u.FavoriteArtists)
                .HasForeignKey(ua => ua.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ActivityLog>()
                .HasOne(al => al.ApplicationUser)
                .WithMany(u => u.RecentActivities)
                .HasForeignKey(al => al.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Event Related Configurations
            modelBuilder.Entity<Event>()
                .HasOne(e => e.Artist)
                .WithMany()
                .HasForeignKey(e => e.ArtistId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<EventAttendee>()
                .HasOne(ea => ea.Event)
                .WithMany(e => e.Attendees)
                .HasForeignKey(ea => ea.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<EventAttendee>()
                .HasOne(ea => ea.ApplicationUser)
                .WithMany(u => u.EventAttendances)
                .HasForeignKey(ea => ea.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Playlist Related Configurations
            modelBuilder.Entity<Playlist>()
                .HasOne(p => p.User)
                .WithMany(u => u.CreatedPlaylists)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PlaylistCollaborator>()
                .HasOne(pc => pc.Playlist)
                .WithMany(p => p.Collaborators)
                .HasForeignKey(pc => pc.PlaylistId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PlaylistCollaborator>()
                .HasOne(pc => pc.ApplicationUser)
                .WithMany(u => u.CollaborativePlaylists)
                .HasForeignKey(pc => pc.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PlaylistSong>()
                .HasOne(ps => ps.Playlist)
                .WithMany(p => p.Songs)
                .HasForeignKey(ps => ps.PlaylistId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PlaylistSong>()
                .HasOne(ps => ps.Song)
                .WithMany(s => s.Playlists)
                .HasForeignKey(ps => ps.SongId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PlaylistSong>()
                .HasOne(ps => ps.User)
                .WithMany(u => u.PlaylistSongs)
                .HasForeignKey(ps => ps.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Song Related Configurations
            modelBuilder.Entity<Song>()
                .HasOne(s => s.Artist)
                .WithMany()
                .HasForeignKey(s => s.ArtistId)
                .OnDelete(DeleteBehavior.Cascade);

            // Chat Related Configurations
            modelBuilder.Entity<ChatMessage>()
                .HasOne(cm => cm.ChatRoom)
                .WithMany(cr => cr.Messages)
                .HasForeignKey(cm => cm.ChatRoomId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ChatMessage>()
                .HasOne(cm => cm.User)
                .WithMany(u => u.ChatMessages)
                .HasForeignKey(cm => cm.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Mood Related Configurations
            modelBuilder.Entity<UserMood>()
                .HasOne(um => um.User)
                .WithMany(u => u.Moods)
                .HasForeignKey(um => um.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserMood>()
                .HasOne(um => um.Mood)
                .WithMany(m => m.UserMoods)
                .HasForeignKey(um => um.MoodId)
                .OnDelete(DeleteBehavior.Cascade);

            // UserMatch Configurations (requires special handling due to dual relationship)
            modelBuilder.Entity<UserMatch>()
                .HasOne(um => um.User1)
                .WithMany(u => u.Matches)
                .HasForeignKey(um => um.User1Id)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure UserChatRoom as an intermediary table
            modelBuilder.Entity<UserChatRoom>()
                .HasKey(uc => uc.Id); // Primary Key for the intermediary table

            // Relationship between ApplicationUser and UserChatRoom
            modelBuilder.Entity<UserChatRoom>()
                .HasOne(uc => uc.User)
                .WithMany(u => u.UserChatRooms)
                .HasForeignKey(uc => uc.UserId);

            // Relationship between ChatRoom and UserChatRoom
            modelBuilder.Entity<UserChatRoom>()
                .HasOne(uc => uc.ChatRoom)
                .WithMany(cr => cr.UserChatRooms)
                .HasForeignKey(uc => uc.ChatRoomId);

            modelBuilder.Entity<Song>()
               .HasOne(s => s.Artist)    //Song has one Artist
               .WithMany(a => a.Songs)  //artist has many Songs
               .HasForeignKey(s => s.ArtistId) 
               .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserPreferencesForm>()
                .HasOne(upf => upf.User)  // UserPreferencesForm has one ApplicationUser
                .WithMany(u => u.Preferences)  // ApplicationUser has many UserPreferencesForms
                .HasForeignKey(upf => upf.UserId)  // Foreign key for UserId
                .OnDelete(DeleteBehavior.Cascade);  // Cascade delete: deleting user deletes related UserPreferencesForm

            // Configure UserPreferencesForm -> User relationship
            modelBuilder.Entity<UserPreferencesForm>()
                .HasOne(upf => upf.User)
                .WithMany(u => u.Preferences)
                .HasForeignKey(upf => upf.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure Song -> Artist relationship
            modelBuilder.Entity<Song>()
                .HasOne(s => s.Artist)
                .WithMany(a => a.Songs)
                .HasForeignKey(s => s.ArtistId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure UserPreferencesSong (junction table)
            modelBuilder.Entity<UserPreferencesSong>()
                .HasKey(ups => new { ups.UserPreferencesFormId, ups.SongId });

            modelBuilder.Entity<UserPreferencesSong>()
                .HasOne(ups => ups.UserPreferencesForm)
                .WithMany(upf => upf.UserPreferencesSongs)
                .HasForeignKey(ups => ups.UserPreferencesFormId);

            modelBuilder.Entity<UserPreferencesSong>()
                .HasOne(ups => ups.Song)
                .WithMany(s => s.UserPreferencesSongs)
                .HasForeignKey(ups => ups.SongId);

            // Configure UserPreferencesArtist (junction table)
            modelBuilder.Entity<UserPreferencesArtist>()
                .HasKey(upa => new { upa.UserPreferencesFormId, upa.ArtistId });

            modelBuilder.Entity<UserPreferencesArtist>()
                .HasOne(upa => upa.UserPreferencesForm)
                .WithMany(upf => upf.UserPreferencesArtists)
                .HasForeignKey(upa => upa.UserPreferencesFormId);

            modelBuilder.Entity<UserPreferencesArtist>()
                .HasOne(upa => upa.Artist)
                .WithMany(a => a.UserPreferencesArtists)
                .HasForeignKey(upa => upa.ArtistId);

            modelBuilder.Entity<Notification>()
           .HasOne(n => n.User)
           .WithMany()
           .HasForeignKey(n => n.UserId)
           .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Notification>()
                .HasOne(n => n.SenderUser)
                .WithMany()
                .HasForeignKey(n => n.SenderUserId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Notification>(entity =>
            {
                entity.Property(e => e.UserId).IsRequired();
                entity.Property(e => e.Title).IsRequired();
                entity.Property(e => e.Message).IsRequired();
                entity.Property(e => e.Type).IsRequired().HasDefaultValue("General");
                entity.Property(e => e.Status).IsRequired().HasDefaultValue("Unread");

                entity.HasOne(n => n.User)
                      .WithMany()
                      .HasForeignKey(n => n.UserId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(n => n.Artist)
                      .WithMany()
                      .HasForeignKey(n => n.ArtistId)
                      .IsRequired(false);

                entity.HasOne(n => n.SenderUser)
                      .WithMany()
                      .HasForeignKey(n => n.SenderUserId)
                      .IsRequired(false);
            });


        }
    }
}
