using FluentAssertions;
using Journey_of_faith.Application.usecases.quizs.commands;
using Journey_of_faith.Domain.interfaces;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace UnitTesting.HandlerTests.quiz
{
    public class DeleteQuizHandlerTest
    {
        private readonly Mock<IExamRepository> _repoMock;
        private readonly DeleteQuizHandler _handler;

        public DeleteQuizHandlerTest()
        {
            _repoMock = new Mock<IExamRepository>();
            _handler = new DeleteQuizHandler(_repoMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldCallDeleteQuizAndReturnTrue_WhenQuizDeleted()
        {
            // Arrange
            int quizId = 1;
            var command = new DeleteQuizCommand { Id = quizId };

            _repoMock.Setup(r => r.DeleteQuiz(quizId)).ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeTrue();
            _repoMock.Verify(r => r.DeleteQuiz(quizId), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnFalse_WhenQuizNotFoundOrNotDeleted()
        {
            // Arrange
            int quizId = 999;
            var command = new DeleteQuizCommand { Id = quizId };

            _repoMock.Setup(r => r.DeleteQuiz(quizId)).ReturnsAsync(false);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeFalse();
            _repoMock.Verify(r => r.DeleteQuiz(quizId), Times.Once);
        }
    }
}
