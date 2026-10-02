using FluentValidation.TestHelper;
using Journey_of_faith.Application.common.interfaces;
using Journey_of_faith.Application.usecases.quizs.commands;
using Journey_of_faith.Domain.interfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace UnitTesting.ValidatorTests
{
    public class CreateQuizCommandValidatorTest
    {
        private readonly CreateQuizValidator _command;
        private readonly Mock<IQuestionRepository> mockQuestionRepository;
        private readonly Mock<ICurrentUserService> mockUserService;
        public CreateQuizCommandValidatorTest()
        {
            mockQuestionRepository = new Mock<IQuestionRepository>();
            mockUserService = new Mock<ICurrentUserService>();
            _command = new CreateQuizValidator(mockQuestionRepository.Object, mockUserService.Object);
        }

        [Fact]
        public async Task Should_Have_Error_When_Title_Is_Empty()
        {
            var createQuizCommand = new CreateQuizCommand
            {
                Title = string.Empty,
            };

            var result = await _command.TestValidateAsync(createQuizCommand);
            result.ShouldHaveValidationErrorFor(e => e.Title);
        }


        [Fact]
        public async Task Should_HaveError_When_Question_Is_Empty()
        {
            var createQuizCommnad = new CreateQuizCommand
            {
                Description = string.Empty,
            };

            var result = await _command.TestValidateAsync(createQuizCommnad);
            result.ShouldHaveValidationErrorFor(e => e.Description);
        }
    }
}
