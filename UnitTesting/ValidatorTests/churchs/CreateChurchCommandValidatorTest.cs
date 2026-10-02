using FluentValidation.TestHelper;
using Journey_of_faith.Application.usecases.churchs.commands;
using Xunit;

namespace UnitTesting.ValidatorTests.churchs
{
    public class CreateChurchCommandValidatorTest
    {
        private readonly CreateChurchCommandValidator _validator;

        public CreateChurchCommandValidatorTest()
        {
            _validator = new CreateChurchCommandValidator();
        }

        [Fact]
        public void Should_Have_Error_When_Name_Is_Empty()
        {
            var command = new CreateChurchCommand { Name = "" };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Name);
        }

        [Fact]
        public void Should_Have_Error_When_DioceseId_Is_Zero_Or_Negative()
        {
            var command = new CreateChurchCommand { DioceseId = 0 };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.DioceseId);
        }

        [Fact]
        public void Should_Have_Error_When_Address_Is_Empty()
        {
            var command = new CreateChurchCommand { Address = "" };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Address);
        }

        [Fact]
        public void Should_Have_Error_When_Latitude_Is_Out_Of_Range()
        {
            var command = new CreateChurchCommand { Latitude = 100 };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Latitude);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Command_Is_Valid()
        {
            var command = new CreateChurchCommand
            {
                Name = "Nhà thờ Tân Định",
                Address = "289 Hai Bà Trưng",
                DioceseId = 1,
                Latitude = 10.7885f,
                Longitude = 106.6908f
            };
            var result = _validator.TestValidate(command);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
