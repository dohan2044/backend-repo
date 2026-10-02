using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Journey_of_faith.Application.usecases.auth.queries
{
    public record UserLoginResponse(bool success, string token, string refreshToken, int expiry);
    public class UserLoginQuery : IRequest<UserLoginResponse>
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }


    public class UserLoginQueryValidation : AbstractValidator<UserLoginQuery>
    {
        public UserLoginQueryValidation()
        {
            RuleFor(e => e.Email)
                .MinimumLength(5).WithMessage("Tên đăng nhập không hợp lệ")
                .NotEmpty().WithMessage("Tên đăng nhập không được bỏ trống");

            RuleFor(e => e.Password)
                .NotEmpty().WithMessage("Mật khẩu không được bỏ trống")
                .MinimumLength(8).WithMessage("Mật khẩu không đủ số ký tự.");
        }
    }
}
