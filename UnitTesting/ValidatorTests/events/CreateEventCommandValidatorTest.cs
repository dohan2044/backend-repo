using FluentValidation.TestHelper;
using Journey_of_faith.Application.usecases.events.commands;
using System;
using System.Collections.Generic;
using Xunit;

namespace UnitTesting.ValidatorTests.events
{
    public class CreateEventCommandValidatorTest
    {
        private readonly CreateEventCommandValidator _validator;

        public CreateEventCommandValidatorTest()
        {
            _validator = new CreateEventCommandValidator();
        }

        [Fact]
        public void Should_Have_Error_When_Title_Is_Empty()
        {
            var command = new CreateEventCommand { Title = "" };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Title);
        }

        [Fact]
        public void Should_Have_Error_When_CategoryIds_Is_Empty()
        {
            var command = new CreateEventCommand { CategoryIds = new List<int>() };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.CategoryIds);
        }

        [Fact]
        public void Should_Have_Error_When_EndDate_Is_Before_StartDate()
        {
            var command = new CreateEventCommand
            {
                Title = "Sự kiện",
                StartDate = DateTime.UtcNow.AddDays(2),
                EndDate = DateTime.UtcNow,
                CategoryIds = new List<int> { 1 }
            };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Command_Is_Valid()
        {
            var command = new CreateEventCommand
            {
                Title = "Sự kiện Giáng Sinh",
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(1),
                CategoryIds = new List<int> { 1 }
            };
            var result = _validator.TestValidate(command);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
