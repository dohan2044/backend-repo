using FluentAssertions;
using Journey_of_faith.Application.common.interfaces;
using Journey_of_faith.Application.exceptions;
using Journey_of_faith.Application.usecases.auth.commands;
using MediatR;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace UnitTesting.HandlerTests.auth
{
    public class ChangePasswordHandlerTest
    {
        private readonly Mock<IAuthService> _authServiceMock;
        private readonly ChangePasswordHandler _handler;

        public ChangePasswordHandlerTest()
        {
            _authServiceMock = new Mock<IAuthService>();
            _handler = new ChangePasswordHandler(_authServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldThrowUnauthorizationException_WhenChangePasswordFails()
        {
            // Arrange
            var command = new ChangePasswordCommand
            {
                CurrentPassword = "OldPassword123",
                NewPassword = "NewPassword123"
            };

            _authServiceMock
                .Setup(s => s.ChangePassword(command.CurrentPassword, command.NewPassword))
                .ReturnsAsync(false);

            // Act & Assert
            var act = async () => await _handler.Handle(command, CancellationToken.None);
            await act.Should().ThrowAsync<UnauthorizationException>()
                .WithMessage("Đổi mật khẩu thất bại vui, lòng nhập đúng mật khẩu");
        }

        [Fact]
        public async Task Handle_ShouldReturnUnitValue_WhenChangePasswordSucceeds()
        {
            // Arrange
            var command = new ChangePasswordCommand
            {
                CurrentPassword = "OldPassword123",
                NewPassword = "NewPassword123"
            };

            _authServiceMock
                .Setup(s => s.ChangePassword(command.CurrentPassword, command.NewPassword))
                .ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().Be(Unit.Value);
            _authServiceMock.Verify(s => s.ChangePassword(command.CurrentPassword, command.NewPassword), Times.Once);
        }
    }
}
