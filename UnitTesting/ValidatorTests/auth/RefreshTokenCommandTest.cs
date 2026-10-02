using FluentValidation.TestHelper;
using Journey_of_faith.Application.common.interfaces;
using Journey_of_faith.Application.usecases.auth.commands;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace UnitTesting.ValidatorTests.auth
{
    public class RefreshTokenCommandTest
    {
        private readonly CreateRefreshTokenValidator _validator;
        public RefreshTokenCommandTest()
        {
            _validator = new CreateRefreshTokenValidator();
        }
        [Fact]
        public void Should_Have_Error_When_RefreshToken_Is_Empty()
        {
            var command = new CreateRefreshTokenCommand { Token = "" };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(e => e.Token);
        }
    }
}
