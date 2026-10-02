using Journey_of_faith.Application.common.interfaces;
using Journey_of_faith.Application.exceptions;
using Journey_of_faith.Application.usecases.questions.commands;
using Journey_of_faith.Domain.entities.quiz;
using Journey_of_faith.Domain.interfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace UnitTesting.HandlerTests.quiz
{
    public class QuestionCreateHandlerTest
    {
        private readonly CreateQuestionHandler _handler;
        private readonly Mock<IQuestionRepository> _mock;
        private readonly Mock<IDbConnectionFactory> _dbConnectionFactoryMock;
        public QuestionCreateHandlerTest()
        {
            _mock = new Mock<IQuestionRepository>();
            _dbConnectionFactoryMock = new Mock<IDbConnectionFactory>();
            _handler = new CreateQuestionHandler(_mock.Object, _dbConnectionFactoryMock.Object);
        }


        [Fact]
        public async Task Handle_ShouldThrowException_WhenCategoryNotExists()
        {
            // arrange 
            _mock.Setup(r => r.CheckValidId(It.IsAny<int>(), It.IsAny<string>()))
                .ThrowsAsync(new NotFoundException("Danh mục câu hỏi không tồn tại."));

            var exception = await Assert.ThrowsAsync<NotFoundException>(async () =>
                await _handler.Handle(new CreateQuestionCommand
                {
                    LevelId = 1,
                    QuestionContent = "Câu hỏi mẫu",
                    TypeId = 1,
                    CategoryId = 1,
                    ImageUrl = "http://example.com/image.jpg",
                    Items = new List<CreateAnswerCommand>
                    {
                        new CreateAnswerCommand
                        {
                            Content = "Đáp án 1",
                            IsCorrect = true,
                            ImageUrl = "http://example.com/answer1.jpg",
                            Explanation = "Giải thích đáp án 1"
                        },
                        new CreateAnswerCommand
                        {
                            Content = "Đáp án 2",
                            IsCorrect = false,
                            ImageUrl = "http://example.com/answer2.jpg",
                            Explanation = "Giải thích đáp án 2"
                        },
                        new CreateAnswerCommand
                        {
                            Content = "Đáp án 3",
                            IsCorrect = false,
                            ImageUrl = "http://example.com/answer3.jpg",
                            Explanation = "Giải thích đáp án 3"
                        }
                    }
                }, CancellationToken.None));

            Assert.Equal("Danh mục câu hỏi không tồn tại.", exception.Message);
        }
        [Fact]
        public async Task Handle_ShouldThrowException_WhenLevelTypeNotExists()
        {
            // arrange 
            _mock.Setup(r => r.CheckValidId(It.IsAny<int>(), It.IsAny<string>()))
                .ThrowsAsync(new NotFoundException("Kiểu câu hỏi không tồn tại."));

            //act & assert
            var exception = await Assert.ThrowsAsync<NotFoundException>(async () =>
                await _handler.Handle(new CreateQuestionCommand
                {
                    LevelId = 1,
                    QuestionContent = "Câu hỏi mẫu",
                    TypeId = 1,
                    CategoryId = 1,
                    ImageUrl = "http://example.com/image.jpg",
                    Items = new List<CreateAnswerCommand>
                    {
                        new CreateAnswerCommand
                        {
                            Content = "Đáp án 1",
                            IsCorrect = true,
                            ImageUrl = "http://example.com/answer1.jpg",
                            Explanation = "Giải thích đáp án 1"
                        },
                        new CreateAnswerCommand
                        {
                            Content = "Đáp án 2",
                            IsCorrect = false,
                            ImageUrl = "http://example.com/answer2.jpg",
                            Explanation = "Giải thích đáp án 2"
                        },
                        new CreateAnswerCommand
                        {
                            Content = "Đáp án 3",
                            IsCorrect = false,
                            ImageUrl = "http://example.com/answer3.jpg",
                            Explanation = "Giải thích đáp án 3"
                        }
                    }
                }, CancellationToken.None)
            );
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenTypeNotExists()
        {
            // arrange 
            _mock.Setup(r => r.CheckValidId(It.IsAny<int>(), It.IsAny<string>()))
                .ThrowsAsync(new NotFoundException("Kiểu câu hỏi không tồn tại."));
            //act & assert
            var exception = await Assert.ThrowsAsync<NotFoundException>(async () =>
                await _handler.Handle(new CreateQuestionCommand
                {
                    LevelId = 1,
                    QuestionContent = "Câu hỏi mẫu",
                    TypeId = 1,
                    CategoryId = 1,
                    ImageUrl = "http://example.com/image.jpg",
                    Items = new List<CreateAnswerCommand>
                    {
                        new CreateAnswerCommand
                        {
                            Content = "Đáp án 1",
                            IsCorrect = false,
                            ImageUrl = "http://example.com/answer1.jpg",
                            Explanation = "Giải thích đáp án 1"
                        },
                        new CreateAnswerCommand
                        {
                            Content = "Đáp án 2",
                            IsCorrect = false,
                            ImageUrl = "http://example.com/answer2.jpg",
                            Explanation = "Giải thích đáp án 2"
                        },
                        new CreateAnswerCommand
                        {
                            Content = "Đáp án 3",
                            IsCorrect = false,
                            ImageUrl = "http://example.com/answer3.jpg",
                            Explanation = "Giải thích đáp án 3"
                        }
                    }
                }, CancellationToken.None)
            );
            Assert.Equal("Kiểu câu hỏi không tồn tại.", exception.Message);
        }


        [Fact]
        public async Task Handle_ShouldThrowException_WhenAnswersLessThan3()
        {
            // arrange 
            _mock.Setup(r => r.CheckValidId(It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            var exception = await Assert.ThrowsAsync<BadRequestException>(async () =>
                await _handler.Handle(new CreateQuestionCommand
                {
                    LevelId = 1,
                    QuestionContent = "Câu hỏi mẫu",
                    TypeId = 1,
                    CategoryId = 1,
                    ImageUrl = "http://example.com/image.jpg",
                    Items = new List<CreateAnswerCommand>
                    {
                        new CreateAnswerCommand
                        {
                            Content = "Đáp án 1",
                            IsCorrect = true,
                            ImageUrl = "http://example.com/answer1.jpg",
                            Explanation = "Giải thích đáp án 1"
                        },
                        new CreateAnswerCommand
                        {
                            Content = "Đáp án 2",
                            IsCorrect = false,
                            ImageUrl = "http://example.com/answer2.jpg",
                            Explanation = "Giải thích đáp án 2"
                        }
                    }
                }, CancellationToken.None)
            );
            Assert.Equal("Phải có ít nhất từ 3 đáp án", exception.Message);
        }


        [Fact]
        public async Task Handle_ShouldThrowException_WhenNoCorrectAnswer()
        {
            // arrange 
            _mock.Setup(r => r.CheckValidId(It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync(true);


            var exception = await Assert.ThrowsAsync<BadRequestException>(async () =>
                await _handler.Handle(new CreateQuestionCommand
                {
                    LevelId = 1,
                    QuestionContent = "Câu hỏi mẫu",
                    TypeId = 1,
                    CategoryId = 1,
                    ImageUrl = "http://example.com/image.jpg",
                    Items = new List<CreateAnswerCommand>
                    {
                        new CreateAnswerCommand
                        {
                            Content = "Đáp án 1",
                            IsCorrect = false,
                            ImageUrl = "http://example.com/answer1.jpg",
                            Explanation = "Giải thích đáp án 1"
                        },
                        new CreateAnswerCommand
                        {
                            Content = "Đáp án 2",
                            IsCorrect = false,
                            ImageUrl = "http://example.com/answer2.jpg",
                            Explanation = "Giải thích đáp án 2"
                        },
                        new CreateAnswerCommand
                        {
                            Content = "Đáp án 3",
                            IsCorrect = false,
                            ImageUrl = "http://example.com/answer3.jpg",
                            Explanation = "Giải thích đáp án 3"
                        }
                    }
                }, CancellationToken.None)
            );
            Assert.Equal("Phải có ít nhất một đáp án đúng", exception.Message);
        }


        [Fact]

        public async Task Handle_ShouldThrowException_WhenContentQuestionExists()
        {
            // arrange 
            _mock.Setup(r => r.CheckValidId(It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync(true);
            _mock.Setup(r => r.CheckUniqueName(It.IsAny<string>()))
                .ReturnsAsync(true);


            var exception = await Assert.ThrowsAsync<UnprocessableEntityException>(async () =>
                await _handler.Handle(new CreateQuestionCommand
                {
                    LevelId = 1,
                    QuestionContent = "Câu hỏi mẫu",
                    TypeId = 1,
                    CategoryId = 1,
                    ImageUrl = "http://example.com/image.jpg",
                    Items = new List<CreateAnswerCommand>
                    {
                        new CreateAnswerCommand
                        {
                            Content = "Đáp án 1",
                            IsCorrect = true,
                            ImageUrl = "http://example.com/answer1.jpg",
                            Explanation = "Giải thích đáp án 1"
                        },
                        new CreateAnswerCommand
                        {
                            Content = "Đáp án 2",
                            IsCorrect = false,
                            ImageUrl = "http://example.com/answer2.jpg",
                            Explanation = "Giải thích đáp án 2"
                        },
                        new CreateAnswerCommand
                        {
                            Content = "Đáp án 3",
                            IsCorrect = false,
                            ImageUrl = "http://example.com/answer3.jpg",
                            Explanation = "Giải thích đáp án 3"
                        }
                    }
                }, CancellationToken.None)
            );



            Assert.Equal("Không thể thêm câu hỏi trùng nội dung", exception.Message);
        }


        [Fact]
        public async Task Handle_ShouldReturnTrue_WhenInputValid()
        {

            // arrange 
            _mock.Setup(r => r.CheckValidId(It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync(true);
            _mock.Setup(r => r.CheckUniqueName(It.IsAny<string>()))
                .ReturnsAsync(false);
            _mock.Setup(r => r.CreateQuestionAsync(It.IsAny<Question>()))
                .ReturnsAsync(true);

            // act & assert
            var result = await _handler.Handle(new CreateQuestionCommand
            {
                LevelId = 1,
                QuestionContent = "Câu hỏi mẫu",
                TypeId = 1,
                CategoryId = 1,
                ImageUrl = "http://example.com/image.jpg",
                Items = new List<CreateAnswerCommand>
                    {
                        new CreateAnswerCommand
                        {
                            Content = "Đáp án 1",
                            IsCorrect = true,
                            ImageUrl = "http://example.com/answer1.jpg",
                            Explanation = "Giải thích đáp án 1"
                        },
                        new CreateAnswerCommand
                        {
                            Content = "Đáp án 2",
                            IsCorrect = false,
                            ImageUrl = "http://example.com/answer2.jpg",
                            Explanation = "Giải thích đáp án 2"
                        },
                        new CreateAnswerCommand
                        {
                            Content = "Đáp án 3",
                            IsCorrect = false,
                            ImageUrl = "http://example.com/answer3.jpg",
                            Explanation = "Giải thích đáp án 3"
                        }
                    }
            }, CancellationToken.None);
            Assert.True(result);
            _mock.Verify(r => r.CreateQuestionAsync(It.IsAny<Question>()), Times.Once);
        }
    }
}
