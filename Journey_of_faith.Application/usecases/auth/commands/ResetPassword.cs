using Journey_of_faith.Application.common.interfaces;
using Journey_of_faith.Application.exceptions;
using MediatR;

namespace Journey_of_faith.Application.usecases.auth.commands;


public class ResetPasswordCommand : IRequest<bool>
{
    public string Email {get; set;}
}

public class ResetPasswordHandler : IRequestHandler<ResetPasswordCommand, bool>
{
    private readonly IAuthService authService;
    public ResetPasswordHandler(IAuthService authService)
    {
        this.authService = authService;
    }

    public async Task<bool> Handle(ResetPasswordCommand command, CancellationToken cancellationToken)
    {
        var result = await authService.ResetPassword(command.Email);
        if(!result)
            throw new UnauthorizationException("Tài khoản email không đúng");

        return result;
    }
}