using FluentAssertions;
using Journey_of_faith.Application.exceptions;
using Journey_of_faith.Application.usecases.quizs.commands;
using Journey_of_faith.Domain.entities.quiz;
using Journey_of_faith.Domain.interfaces;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace UnitTesting.HandlerTests.quiz
{
    public class CreateTopicHandlerTest
    {
        private readonly Mock<IExamRepository> _examRepoMock;
        private readonly CreateTopicHandler _handler;

        public CreateTopicHandlerTest()
        {
            _examRepoMock = new Mock<IExamRepository>();
            _handler = new CreateTopicHandler(_examRepoMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldThrowConfictException_WhenTopicNameAlreadyExists()
        {
            // Arrange
            _examRepoMock.Setup(r => r.ExistsNameAsync("Kinh Thánh")).ReturnsAsync(true);

            var command = new CreateTopicCommand
            {
                TopicName = "Kinh Thánh",
                QuizCount = 5
            };

            // Act & Assert
            var act = async () => await _handler.Handle(command, CancellationToken.None);
            await act.Should().ThrowAsync<ConfictException>()
                .WithMessage("Tên chủ đề đã tồn tại");
        }

        [Fact]
        public async Task Handle_ShouldReturnTopicId_WhenDataIsValid()
        {
            // Arrange
            _examRepoMock.Setup(r => r.ExistsNameAsync("Kinh Thánh")).ReturnsAsync(false);
            _examRepoMock.Setup(r => r.CreateTopicAsync(It.IsAny<Topic>())).ReturnsAsync(1);

            var command = new CreateTopicCommand
            {
                TopicName = "Kinh Thánh",
                QuizCount = 5
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().Be(1);
            _examRepoMock.Verify(r => r.CreateTopicAsync(It.IsAny<Topic>()), Times.Once);
        }
    }
}
