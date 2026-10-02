using FluentAssertions;
using Journey_of_faith.Application.common.interfaces;
using Journey_of_faith.Application.exceptions;
using Journey_of_faith.Application.usecases.churchs.commands;
using Journey_of_faith.Domain.entities.location;
using Journey_of_faith.Domain.interfaces;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace UnitTesting.HandlerTests.churchs
{
    public class CreateDioceseHandlerTest
    {
        private readonly Mock<IChurchRepository> _churchRepoMock;
        private readonly Mock<ICurrentUserService> _currentUserServiceMock;
        private readonly CreateDioceseHandler _handler;

        public CreateDioceseHandlerTest()
        {
            _churchRepoMock = new Mock<IChurchRepository>();
            _currentUserServiceMock = new Mock<ICurrentUserService>();
            _handler = new CreateDioceseHandler(_churchRepoMock.Object, _currentUserServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldThrowUnprocessableEntityException_WhenDioceseNameAlreadyExists()
        {
            // Arrange
            _churchRepoMock.Setup(r => r.UniqueNameDiocese("Giáo phận Sài Gòn")).ReturnsAsync(true);

            var command = new CreateDioceseCommand
            {
                Name = "Giáo phận Sài Gòn"
            };

            // Act & Assert
            var act = async () => await _handler.Handle(command, CancellationToken.None);
            await act.Should().ThrowAsync<UnprocessableEntityException>()
                .WithMessage("Tên Giáo xữ đã tòn tại");
        }

        [Fact]
        public async Task Handle_ShouldThrowUnauthorizationException_WhenUserIdIsInvalidGuid()
        {
            // Arrange
            _churchRepoMock.Setup(r => r.UniqueNameDiocese("Giáo phận Sài Gòn")).ReturnsAsync(false);
            _currentUserServiceMock.Setup(s => s.UserId).Returns("not-a-guid");

            var command = new CreateDioceseCommand
            {
                Name = "Giáo phận Sài Gòn"
            };

            // Act & Assert
            var act = async () => await _handler.Handle(command, CancellationToken.None);
            await act.Should().ThrowAsync<UnauthorizationException>()
                .WithMessage("Người dùng không hợp lệ");
        }

        [Fact]
        public async Task Handle_ShouldReturnDioceseId_WhenDataIsValid()
        {
            // Arrange
            var validUserId = Guid.NewGuid().ToString();
            _churchRepoMock.Setup(r => r.UniqueNameDiocese("Giáo phận Sài Gòn")).ReturnsAsync(false);
            _currentUserServiceMock.Setup(s => s.UserId).Returns(validUserId);
            _churchRepoMock.Setup(r => r.CreateAsync(It.IsAny<Diocese>())).ReturnsAsync(5);

            var command = new CreateDioceseCommand
            {
                Name = "Giáo phận Sài Gòn",
                Website = "https://saigon.org",
                Address = "Sài Gòn"
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().Be(5);
            _churchRepoMock.Verify(r => r.CreateAsync(It.IsAny<Diocese>()), Times.Once);
        }
    }
}
