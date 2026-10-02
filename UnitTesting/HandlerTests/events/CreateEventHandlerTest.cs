// using FluentAssertions;
// using Journey_of_faith.Application.common.interfaces;
// using Journey_of_faith.Application.exceptions;
// using Journey_of_faith.Application.usecases.events.commands;
// using Journey_of_faith.Domain.interfaces;
// using Moq;
// using System;
// using System.Collections.Generic;
// using System.Threading;
// using System.Threading.Tasks;
// using Xunit;

// namespace UnitTesting.HandlerTests.events
// {
//     public class CreateEventHandlerTest
//     {
//         private readonly Mock<IEventRepository> _eventRepoMock;
//         private readonly Mock<ICurrentUserService> _currentUserServiceMock;
//         private readonly CreateEventHandler _handler;

//         public CreateEventHandlerTest()
//         {
//             _eventRepoMock = new Mock<IEventRepository>();
//             _currentUserServiceMock = new Mock<ICurrentUserService>();
//             _handler = new CreateEventHandler(_eventRepoMock.Object, _currentUserServiceMock.Object);
//         }

//         [Fact]
//         public async Task Handle_ShouldThrowUnauthorizationException_WhenUserIdIsInvalid()
//         {
//             // Arrange
//             _currentUserServiceMock.Setup(s => s.UserId).Returns("invalid-guid");

//             var command = new CreateEventCommand
//             {
//                 Title = "Hội Trại Giới Trẻ",
//                 CategoryIds = new List<int> { 1 }
//             };

//             // Act & Assert
//             var act = async () => await _handler.Handle(command, CancellationToken.None);
//             await act.Should().ThrowAsync<UnauthorizationException>()
//                 .WithMessage("Không xác định được người dùng hiện tại.");
//         }

//         [Fact]
//         public async Task Handle_ShouldThrowForbiddenException_WhenUserIsNotAdmin()
//         {
//             // Arrange
//             var validUserId = Guid.NewGuid().ToString();
//             _currentUserServiceMock.Setup(s => s.UserId).Returns(validUserId);
//             _currentUserServiceMock.Setup(s => s.GetRoleUserName).Returns("User");

//             var command = new CreateEventCommand
//             {
//                 Title = "Hội Trại Giới Trẻ",
//                 CategoryIds = new List<int> { 1 }
//             };

//             // Act & Assert
//             var act = async () => await _handler.Handle(command, CancellationToken.None);
//             await act.Should().ThrowAsync<ForbiddenException>()
//                 .WithMessage("Bạn không có quyền tạo sự kiện.");
//         }

//         [Fact]
//         public async Task Handle_ShouldThrowNotFoundException_WhenCategoryDoesNotExist()
//         {
//             // Arrange
//             var validUserId = Guid.NewGuid().ToString();
//             _currentUserServiceMock.Setup(s => s.UserId).Returns(validUserId);
//             _currentUserServiceMock.Setup(s => s.GetRoleUserName).Returns("admin");
//             _eventRepoMock.Setup(r => r.CategoryExistsAsync(999)).ReturnsAsync(false);

//             var command = new CreateEventCommand
//             {
//                 Title = "Hội Trại Giới Trẻ",
//                 CategoryIds = new List<int> { 999 }
//             };

//             // Act & Assert
//             var act = async () => await _handler.Handle(command, CancellationToken.None);
//             await act.Should().ThrowAsync<NotFoundException>()
//                 .WithMessage("Không tìm thấy danh mục sự kiện với Id = 999.");
//         }

//         [Fact]
//         public async Task Handle_ShouldReturnEventId_WhenDataIsValid()
//         {
//             // Arrange
//             var validUserId = Guid.NewGuid().ToString();
//             _currentUserServiceMock.Setup(s => s.UserId).Returns(validUserId);
//             _currentUserServiceMock.Setup(s => s.GetRoleUserName).Returns("admin");
//             _eventRepoMock.Setup(r => r.CategoryExistsAsync(1)).ReturnsAsync(true);
//             _eventRepoMock.Setup(r => r.CreateEventAsync(It.IsAny<CreateEventPayload>())).ReturnsAsync(200);

//             var command = new CreateEventCommand
//             {
//                 Title = "Hội Trại Giới Trẻ",
//                 StartDate = DateTime.UtcNow,
//                 CategoryIds = new List<int> { 1 }
//             };

//             // Act
//             var result = await _handler.Handle(command, CancellationToken.None);

//             // Assert
//             result.Should().Be(200);
//             _eventRepoMock.Verify(r => r.CreateEventAsync(It.IsAny<CreateEventPayload>()), Times.Once);
//         }
//     }
// }
using FluentAssertions;
using Journey_of_faith.Application.common.interfaces;
using Journey_of_faith.Application.exceptions;
using Journey_of_faith.Application.usecases.events.commands;
using Journey_of_faith.Domain.interfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace UnitTesting.HandlerTests.events
{
    public class CreateEventHandlerTest
    {
        private readonly Mock<IEventRepository> _eventRepoMock;
        private readonly Mock<ICurrentUserService> _currentUserServiceMock;
        private readonly CreateEventHandler _handler;

        public CreateEventHandlerTest()
        {
            _eventRepoMock = new Mock<IEventRepository>();
            _currentUserServiceMock = new Mock<ICurrentUserService>();
            _handler = new CreateEventHandler(_eventRepoMock.Object, _currentUserServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldThrowUnauthorizationException_WhenUserIdIsInvalid()
        {
            _currentUserServiceMock.Setup(s => s.UserId).Returns("invalid-guid");
            var command = new CreateEventCommand { Title = "Hội Trại Giới Trẻ", CategoryIds = new List<int> { 1 } };
            var act = async () => await _handler.Handle(command, CancellationToken.None);
            await act.Should().ThrowAsync<UnauthorizationException>().WithMessage("Không xác định được người dùng hiện tại.");
        }

        [Fact]
        public async Task Handle_ShouldThrowForbiddenException_WhenUserIsNotAdmin()
        {
            var validUserId = Guid.NewGuid().ToString();
            _currentUserServiceMock.Setup(s => s.UserId).Returns(validUserId);
            _currentUserServiceMock.Setup(s => s.GetRoleUserName).Returns("User");
            var command = new CreateEventCommand { Title = "Hội Trại Giới Trẻ", CategoryIds = new List<int> { 1 } };
            var act = async () => await _handler.Handle(command, CancellationToken.None);
            await act.Should().ThrowAsync<ForbiddenException>().WithMessage("Bạn không có quyền tạo sự kiện.");
        }

        [Fact]
        public async Task Handle_ShouldThrowNotFoundException_WhenCategoryDoesNotExist()
        {
            var validUserId = Guid.NewGuid().ToString();
            _currentUserServiceMock.Setup(s => s.UserId).Returns(validUserId);
            _currentUserServiceMock.Setup(s => s.GetRoleUserName).Returns("admin");
            _eventRepoMock.Setup(r => r.CategoryExistsAsync(999)).ReturnsAsync(false);
            var command = new CreateEventCommand { Title = "Hội Trại Giới Trẻ", CategoryIds = new List<int> { 999 } };
            var act = async () => await _handler.Handle(command, CancellationToken.None);
            await act.Should().ThrowAsync<NotFoundException>().WithMessage("Không tìm thấy danh mục sự kiện với Id = 999.");
        }

        [Fact]
        public async Task Handle_ShouldReturnEventId_WhenDataIsValid()
        {
            var validUserId = Guid.NewGuid().ToString();
            _currentUserServiceMock.Setup(s => s.UserId).Returns(validUserId);
            _currentUserServiceMock.Setup(s => s.GetRoleUserName).Returns("admin");
            _eventRepoMock.Setup(r => r.CategoryExistsAsync(1)).ReturnsAsync(true);
            
            // ✅ SỬA: Dùng string thay vì CreateEventPayload
            _eventRepoMock.Setup(r => r.CreateEventAsync(It.IsAny<string>())).ReturnsAsync(200);

            var command = new CreateEventCommand
            {
                Title = "Hội Trại Giới Trẻ",
                StartDate = DateTime.UtcNow,
                CategoryIds = new List<int> { 1 }
            };

            var result = await _handler.Handle(command, CancellationToken.None);
            result.Should().Be(200);
            _eventRepoMock.Verify(r => r.CreateEventAsync(It.IsAny<string>()), Times.Once);
        }
    }
}