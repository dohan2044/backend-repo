using FluentValidation.TestHelper;
using Journey_of_faith.Application.usecases.auth.queries;
using System;
using System.Collections.Generic;
using System.Text;

namespace UnitTesting.ValidatorTests.auth
{
    public class LoginCommandValidatorTest
    {
        private readonly UserLoginQueryValidation _validator;
        public LoginCommandValidatorTest()
        {
            _validator = new UserLoginQueryValidation();
        }


        [Fact]
        public void Should_Have_Error_When_Email_Is_Empty()
        {
            var command = new UserLoginQuery { Email = "" };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(e => e.Email);
        }


        [Fact]
        public void Should_Have_Error_When_Email_Is_Short()
        {
            var command = new UserLoginQuery { Email = "abc" };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(e => e.Email);
        }

        [Fact]
        public void Should_Have_Error_When_Password_Is_Empty()
        {
            var command = new UserLoginQuery { Password = "" };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(e => e.Password);
        }


        [Fact]
        public void Should_Have_Error_When_Password_Is_Too_Short()
        {
            var command = new UserLoginQuery { Password = "P1a" };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(e => e.Password);
        }
    }
}
