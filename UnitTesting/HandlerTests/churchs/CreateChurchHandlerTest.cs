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
    public class CreateChurchHandlerTest
    {
        private readonly Mock<IChurchRepository> _churchRepoMock;
        private readonly Mock<ICurrentUserService> _currentUserServiceMock;
        private readonly CreateChurchHandler _handler;

        public CreateChurchHandlerTest()
        {
            _churchRepoMock = new Mock<IChurchRepository>();
            _currentUserServiceMock = new Mock<ICurrentUserService>();
            _handler = new CreateChurchHandler(_churchRepoMock.Object, _currentUserServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldThrowUnauthorizationException_WhenUserIdIsInvalid()
        {
            // Arrange
            _currentUserServiceMock.Setup(s => s.UserId).Returns("invalid-guid");

            var command = new CreateChurchCommand
            {
                Name = "Nhà thờ Đức Bà",
                DioceseId = 1
            };

            // Act & Assert
            var act = async () => await _handler.Handle(command, CancellationToken.None);
            await act.Should().ThrowAsync<UnauthorizationException>()
                .WithMessage("Người dùng không hợp lệ");
        }

        [Fact]
        public async Task Handle_ShouldThrowNotFoundException_WhenDioceseDoesNotExist()
        {
            // Arrange
            var validUserId = Guid.NewGuid().ToString();
            _currentUserServiceMock.Setup(s => s.UserId).Returns(validUserId);
            _churchRepoMock.Setup(r => r.GetDioceseExistsAsync(It.IsAny<int>())).ReturnsAsync(false);

            var command = new CreateChurchCommand
            {
                Name = "Nhà thờ Đức Bà",
                DioceseId = 999
            };

            // Act & Assert
            var act = async () => await _handler.Handle(command, CancellationToken.None);
            await act.Should().ThrowAsync<NotFoundException>()
                .WithMessage("Không có giáo phận mà nhà nhờ đăng ký.");
        }

        [Fact]
        public async Task Handle_ShouldReturnChurchId_WhenDataIsValid()
        {
            // Arrange
            var validUserId = Guid.NewGuid().ToString();
            _currentUserServiceMock.Setup(s => s.UserId).Returns(validUserId);
            _churchRepoMock.Setup(r => r.GetDioceseExistsAsync(1)).ReturnsAsync(true);
            _churchRepoMock.Setup(r => r.CreateAsync(It.IsAny<Church>())).ReturnsAsync(10);

            var command = new CreateChurchCommand
            {
                Name = "Nhà thờ Đức Bà",
                DioceseId = 1,
                Address = "1 Công xã Paris",
                Latitude = 10.77978f,
                Longitude = 106.69901f
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().Be(10);
            _churchRepoMock.Verify(r => r.CreateAsync(It.IsAny<Church>()), Times.Once);
        }
    }
}
