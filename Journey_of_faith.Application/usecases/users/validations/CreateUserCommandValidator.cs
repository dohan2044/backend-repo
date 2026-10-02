using FluentValidation;
using Journey_of_faith.Application.usecases.users.commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace Journey_of_faith.Application.usecases.users.validations
{
    public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
    {
        public CreateUserCommandValidator() {
            RuleFor(u => u.Username)
                .NotEmpty().WithMessage("Tên Đăng nhập không được bỏ trống")
                .MinimumLength(6).WithMessage("Tên đăng nhập không được nhỏ hơn 6 ký tự");

            RuleFor(u => u.Password)
                .NotEmpty().WithMessage("Mật khẩu không được bỏ trống")
                .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$")
            .WithMessage("Mật khẩu phải ít nhất 8 ký tự, có chữ hoa, chữ thường và số.");


            RuleFor(u => u.Email)
                .NotEmpty().WithMessage("Email không được bỏ trống")
                .EmailAddress().WithMessage("Email không đúng định dạng.");
        }
    }
}
