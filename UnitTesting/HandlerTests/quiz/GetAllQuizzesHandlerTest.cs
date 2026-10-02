using Journey_of_faith.Application.usecases.quizs.queries;
using Journey_of_faith.Application.usecases.quizs;
using Journey_of_faith.Application.common.dtos.quiz;
using Moq;

namespace UnitTesting.HandlerTests.quiz
{
    public class GetAllQuizzesHandlerTest
    {
        [Fact]
        public async Task Handle_ShouldReturnAllQuizzesFromRepository()
        {
            var quizzes = new List<QuizViewDto>
            {
                new()
                {
                    Id = 2,
                    Title = "Đề thi 2",
                    Questions = new List<QuizQuestionDto>
                    {
                        new()
                        {
                            Id = 10,
                            QuestionContent = "Câu hỏi 1",
                            Answers = new List<QuizAnswerDto>
                            {
                                new() { Id = 100, QuestionId = 10, Content = "Đáp án 1" }
                            }
                        }
                    }
                },
                new() { Id = 1, Title = "Đề thi 1" }
            };
            var queries = new Mock<IExamQueries>();
            queries.Setup(x => x.GetAllQuizzesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(quizzes);
            var handler = new GetAllQuizzesHandler(queries.Object);

            var result = await handler.Handle(new GetAllQuizzesQuery(), CancellationToken.None);

            Assert.Equal(quizzes, result);
            queries.Verify(x => x.GetAllQuizzesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
