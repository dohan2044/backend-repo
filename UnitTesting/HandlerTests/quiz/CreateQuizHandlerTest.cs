using FluentAssertions;
using Journey_of_faith.Application.exceptions;
using Journey_of_faith.Application.usecases.quizs.commands;
using Journey_of_faith.Domain.entities.quiz;
using Journey_of_faith.Domain.interfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace UnitTesting.HandlerTests.quiz
{
    public class CreateQuizHandlerTest
    {
        private readonly CreateQuizHandler _handler;
        private readonly Mock<IExamRepository> _mockExam;
        private readonly Mock<IQuestionRepository> _mockQuestion;
        public CreateQuizHandlerTest()
        {
            _mockExam = new Mock<IExamRepository>();
            _mockQuestion = new Mock<IQuestionRepository>();
            _handler = new CreateQuizHandler(_mockExam.Object, _mockQuestion.Object);
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenTotalCountQuestionLessThanInputCountQuestion()
        {
            // arrange
            _mockQuestion.Setup(e => e.GetCountQuestion())
                .ReturnsAsync(2);

            // act & assert
            var command = new CreateQuizCommand
            {
                Title = "Test",
                Description = "Test",
                IsDaily = true,
                TimeLimit = 10,
                QuestionCount = 3,
                HardQuestion = 1,
                MediumQuestion = 1,
                EasyQuestion = 1
            };

            var exception = await Assert.ThrowsAsync<BadRequestException>( async () =>
               await _handler.Handle(command, CancellationToken.None)
            );

            Assert.Equal("Không đủ số câu hỏi để tạo đề thi", exception.Message);
        }


        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenInputValid()
        {
            // arrange 
            var command = new CreateQuizCommand
            {
                Title = "Test",
                Description = "Test",
                IsDaily = true,
                TimeLimit = 10,
                QuestionCount = 10,
                HardQuestion = 2,
                MediumQuestion = 3,
                EasyQuestion = 5
            };

            _mockExam.Setup(e => e.CreateQuiz(It.IsAny<Quiz>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(1);

            _mockQuestion.Setup(e => e.GetCountQuestion())
                .ReturnsAsync(100);
            // act
            var result = await _handler.Handle(command, CancellationToken.None);

            // assert
            Assert.Equal(1, result);

            _mockExam
                .Verify(r => r.CreateQuiz(It.IsAny<Quiz>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()), times: Times.Once);
        }
    }
}
