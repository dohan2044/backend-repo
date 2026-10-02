using FluentValidation.TestHelper;
using Journey_of_faith.Application.usecases.users.commands;
using Journey_of_faith.Application.usecases.users.validations;
using System;
using System.Collections.Generic;
using System.Text;

namespace UnitTesting.ValidatorTests.users
{
    public class CreateUserCommandValidatorTest
    {

        private readonly CreateUserCommandValidator _validator;
        public CreateUserCommandValidatorTest()
        {
            _validator = new CreateUserCommandValidator();
        }


        [Fact]
        public void Should_Have_Error_When_Username_Is_Empty()
        {
            var user = new CreateUserCommand { Username = "" };

            var result = _validator.TestValidate(user);

            result.ShouldHaveValidationErrorFor(e => e.Username);
        }
        [Fact]
        public void Should_Have_Error_When_Username_Is_Short()
        {
            var user = new CreateUserCommand { Username = "abc" };
            var result = _validator.TestValidate(user);
            result.ShouldHaveValidationErrorFor(e => e.Username);
        }


        [Fact]
        public void Should_Have_Error_When_Password_Is_Empty()
        {
            var user = new CreateUserCommand { Password = "" };
            var result = _validator.TestValidate(user);
            result.ShouldHaveValidationErrorFor(e => e.Password);
        }

        [Fact]
        public void Should_Have_Error_When_Password_Is_Weak()
        {
            var user = new CreateUserCommand { Password = "password" };
            var result = _validator.TestValidate(user);
            result.ShouldHaveValidationErrorFor(e => e.Password);
        }


        [Fact]
        public void Should_Have_Error_When_Password_Is_Too_Short()
        {
            var user = new CreateUserCommand { Password = "P1a" };
            var result = _validator.TestValidate(user);
            result.ShouldHaveValidationErrorFor(e => e.Password);
        }

        [Fact]
        public void Should_Have_Error_When_Email_Is_Empty()
        {
            var user = new CreateUserCommand { Email = "" };
            var result = _validator.TestValidate(user);
            result.ShouldHaveValidationErrorFor(e => e.Email);
        }


        [Fact]
        public void Should_Have_Error_When_Email_Is_Invalid()
        {
            var user = new CreateUserCommand { Email = "invalid-email" };
            var result = _validator.TestValidate(user);
            result.ShouldHaveValidationErrorFor(e => e.Email);
        }
    }
}
