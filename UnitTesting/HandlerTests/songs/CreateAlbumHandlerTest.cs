using FluentAssertions;
using Journey_of_faith.Application.usecases.songs.commands;
using Journey_of_faith.Domain.entities.musics;
using Journey_of_faith.Domain.interfaces;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace UnitTesting.HandlerTests.songs
{
    public class CreateAlbumHandlerTest
    {
        private readonly Mock<ISongRepository> _songRepoMock;
        private readonly CreateAlbumHandler _handler;

        public CreateAlbumHandlerTest()
        {
            _songRepoMock = new Mock<ISongRepository>();
            _handler = new CreateAlbumHandler(_songRepoMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldCreateAlbumAndReturnId_WhenDataIsValid()
        {
            // Arrange
            var command = new CreateAlbumCommand
            {
                Title = "Album Thánh Ca",
                ArtistId = 1,
                ReleaseYear = 2024,
                CoverImageUrl = "https://example.com/cover.jpg"
            };

            _songRepoMock
                .Setup(r => r.CreateAlbumAsync(It.IsAny<Album>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(50);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().Be(50);
            _songRepoMock.Verify(r => r.CreateAlbumAsync(It.IsAny<Album>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
