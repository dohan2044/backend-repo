using FluentValidation.TestHelper;
using Journey_of_faith.Application.usecases.songs.commands;
using Journey_of_faith.Domain.interfaces;
using Moq;
using Xunit;

namespace UnitTesting.ValidatorTests.songs
{
    public class CreateSongValidatorTest
    {
        private readonly CreateSongValidator _validator;
        private readonly Mock<ISongRepository> _songRepoMock;

        public CreateSongValidatorTest()
        {
            _songRepoMock = new Mock<ISongRepository>();
            _validator = new CreateSongValidator(_songRepoMock.Object);
        }

        [Fact]
        public void Should_Have_Error_When_Title_Is_Empty()
        {
            var command = new CreateSongCommand { Title = "" };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(e => e.Title);
        }

        [Fact]
        public void Should_Have_Error_When_ArtistId_Is_Zero()
        {
            var command = new CreateSongCommand { ArtistId = 0 };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(e => e.ArtistId);
        }

        [Fact]
        public void Should_Have_Error_When_AlbumId_Is_Zero()
        {
            var command = new CreateSongCommand { AlbumId = 0 };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(e => e.AlbumId);
        }

        [Fact]
        public void Should_Have_Error_When_AudioUrl_Is_Empty()
        {
            var command = new CreateSongCommand { AudioUrl = "" };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(e => e.AudioUrl);
        }
    }
}
