# MusicMatch Application Testing

## Technology Used for Testing

The testing suite for **MusicMatch** is based on **Xunit**, a popular testing framework for .NET applications. Xunit provides a rich set of features to test various components of the application, from individual entity creation to the validation of user workflows. This ensures the stability, performance, and correctness of the application.

Key features of **Xunit**:
- **Test case organization**: Xunit allows tests to be organized within classes and attributes like `[Fact]` for individual test methods.
- **Assertions**: Assertions help validate the correctness of entities and ensure they behave as expected.
- **Test runners**: Xunit integrates well with .NET Core and Visual Studio to run tests effectively.

## Running Tests

We grouped the creation of multiple entities into a single file EntityTests.cs which you can see below:

```csharp

using Xunit;
using MusicMatch.Models;
using System;

namespace MusicMatch.Tests
{
    public class EntityTests
    {
        [Fact]
        public void CreateArtist_WithValidData_ReturnsSuccess()
        {
            // Arrange
            var artist = new Artist
            {
                Name = "Test Artist",
                Bio = "Test Bio",
                PhotoUrl = "test-photo.jpg"
            };

            // Assert
            Assert.NotNull(artist);
            Assert.Equal("Test Artist", artist.Name);
            Assert.Equal("Test Bio", artist.Bio);
        }

        [Fact]
        public void CreateEvent_WithValidData_ReturnsSuccess()
        {
            // Arrange
            var eventDate = DateTime.Now.AddDays(10);
            var eventObj = new Event
            {
                Name = "Test Event",
                Description = "Test Description",
                DateTime = eventDate,
                Location = "Test Location",
                Type = "Concert"
            };

            // Assert
            Assert.NotNull(eventObj);
            Assert.Equal("Test Event", eventObj.Name);
            Assert.Equal("Concert", eventObj.Type);
            Assert.Equal(eventDate.Date, eventObj.DateTime.Date);
        }

        [Fact]
        public void CreateGenre_WithValidData_ReturnsSuccess()
        {
            // Arrange
            var genre = new Genre
            {
                Name = "Test Genre",
                Description = "Test Description"
            };

            // Assert
            Assert.NotNull(genre);
            Assert.Equal("Test Genre", genre.Name);
        }

        [Fact]
        public void CreatePlaylist_WithValidData_ReturnsSuccess()
        {
            // Arrange
            var playlist = new Playlist
            {
                Name = "Test Playlist",
                Description = "Test Description",
                Mood = "Happy",
                Genre = "Pop",
                IsCollaborative = true,
                Visibility = "Public"
            };

            // Assert
            Assert.NotNull(playlist);
            Assert.Equal("Test Playlist", playlist.Name);
            Assert.True(playlist.IsCollaborative);
            Assert.Equal("Public", playlist.Visibility);
        }

        [Fact]
        public void CreateSong_WithValidData_ReturnsSuccess()
        {
            // Arrange
            var song = new Song
            {
                Title = "Test Song",
                ArtistId = 1,
                GenreId = 1,
                Mood = "Happy",
                Duration = TimeSpan.FromMinutes(3),
                ReleaseDate = DateTime.Now
            };

            // Assert
            Assert.NotNull(song);
            Assert.Equal("Test Song", song.Title);
            Assert.Equal(TimeSpan.FromMinutes(3), song.Duration);
        }
    }
}
