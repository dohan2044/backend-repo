using FluentAssertions;
using Journey_of_faith.Application.exceptions;
using Journey_of_faith.Application.usecases.auth.commands;
using Journey_of_faith.Domain.interfaces;
using Moq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace UnitTesting.HandlerTests.auth
{
    public class AddPermissionHandlerTest
    {
        private readonly Mock<IRoleRepository> _roleRepositoryMock;
        private readonly AddPermissionHandler _handler;

        public AddPermissionHandlerTest()
        {
            _roleRepositoryMock = new Mock<IRoleRepository>();
            _handler = new AddPermissionHandler(_roleRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldThrowBadRequestException_WhenRoleNameIsEmpty()
        {
            // Arrange
            var command = new AddPermissionCommand
            {
                RoleName = "",
                Permissions = new List<string> { "Read" }
            };

            // Act & Assert
            var act = async () => await _handler.Handle(command, CancellationToken.None);
            await act.Should().ThrowAsync<BadRequestException>()
                .WithMessage("Tên vai trò không được để trống");
        }

        [Fact]
        public async Task Handle_ShouldThrowBadRequestException_WhenPermissionsIsEmpty()
        {
            // Arrange
            var command = new AddPermissionCommand
            {
                RoleName = "Admin",
                Permissions = new List<string>()
            };

            // Act & Assert
            var act = async () => await _handler.Handle(command, CancellationToken.None);
            await act.Should().ThrowAsync<BadRequestException>()
                .WithMessage("Danh sách vai trò trống");
        }

        [Fact]
        public async Task Handle_ShouldReturnTrue_WhenDataIsValid()
        {
            // Arrange
            var command = new AddPermissionCommand
            {
                RoleName = "Admin",
                Permissions = new List<string> { "Read", "Write" }
            };

            _roleRepositoryMock
                .Setup(r => r.AddPermissionForRole(command.RoleName, command.Permissions))
                .ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeTrue();
            _roleRepositoryMock.Verify(r => r.AddPermissionForRole(command.RoleName, command.Permissions), Times.Once);
        }
    }
}
