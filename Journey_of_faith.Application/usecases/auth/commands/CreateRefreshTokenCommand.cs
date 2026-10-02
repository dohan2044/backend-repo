using FluentValidation;
using Journey_of_faith.Application.common.interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Journey_of_faith.Application.usecases.auth.commands
{
    public class CreateRefreshTokenCommand : IRequest<LoginUserResponse>
    {
        public string Token { get; set; } = string.Empty;
    }


    public class CreateRefreshTokenValidator : AbstractValidator<CreateRefreshTokenCommand>
    {
        public CreateRefreshTokenValidator()
        {
            RuleFor(x => x.Token).NotEmpty().WithMessage("Token không được để trống");
        }
    }
}
