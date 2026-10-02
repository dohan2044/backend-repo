using FluentAssertions;
using Journey_of_faith.Application.exceptions;
using Journey_of_faith.Application.usecases.songs.commands;
using Journey_of_faith.Domain.entities.musics;
using Journey_of_faith.Domain.interfaces;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace UnitTesting.HandlerTests.songs
{
    public class CreateSongHandlerTest
    {
        private readonly Mock<ISongRepository> _songRepoMock;
        private readonly CreateSongHandler _handler;

        public CreateSongHandlerTest()
        {
            _songRepoMock = new Mock<ISongRepository>();
            _handler = new CreateSongHandler(_songRepoMock.Object);
        }
        [Fact]
        public async Task Handle_ShouldThrowNotFoundException_WhenArtistDoesNotExist()
        {
            // Arrange
            _songRepoMock.Setup(r => r.ExitsArtistAsync(1)).ReturnsAsync(false);

            var command = new CreateSongCommand
            {
                Title = "Bài Ca Hy Vọng",
                ArtistId = 1,
                AlbumId = 1,
                Duration = 200,
                AudioUrl = "https://example.com/audio.mp3",
                CategorySongId = 1
            };

            // Act & Assert
            var act = async () => await _handler.Handle(command, CancellationToken.None);
            await act.Should().ThrowAsync<NotFoundException>()
                .WithMessage("Nghệ sĩ không tồn tại");
        }

        [Fact]
        public async Task Handle_ShouldThrowNotFoundException_WhenAlbumDoesNotExist()
        {
            // Arrange
            _songRepoMock.Setup(r => r.ExitsArtistAsync(1)).ReturnsAsync(true);
            _songRepoMock.Setup(r => r.ExitsAlbumAsync(2)).ReturnsAsync(false);

            var command = new CreateSongCommand
            {
                Title = "Bài Ca Hy Vọng",
                ArtistId = 1,
                AlbumId = 2,
                Duration = 200,
                AudioUrl = "https://example.com/audio.mp3",
                CategorySongId = 1
            };

            // Act & Assert
            var act = async () => await _handler.Handle(command, CancellationToken.None);
            await act.Should().ThrowAsync<NotFoundException>()
                .WithMessage("Album không tồn tại.");
        }

        [Fact]
        public async Task Handle_ShouldReturnSongId_WhenDataIsValid()
        {
            // Arrange
            _songRepoMock.Setup(r => r.ExitsArtistAsync(1)).ReturnsAsync(true);
            _songRepoMock.Setup(r => r.ExitsAlbumAsync(1)).ReturnsAsync(true);
            _songRepoMock.Setup(r => r.CreateSongAsync(It.IsAny<Song>(), 1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(100);

            var command = new CreateSongCommand
            {
                Title = "Bài Ca Hy Vọng",
                ArtistId = 1,
                AlbumId = 1,
                Duration = 200,
                AudioUrl = "https://example.com/audio.mp3",
                CategorySongId = 1
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().Be(100);
            _songRepoMock.Verify(r => r.CreateSongAsync(It.IsAny<Song>(), 1, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
