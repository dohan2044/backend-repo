using Journey_of_faith.Application.common.interfaces;
using Journey_of_faith.Application.exceptions;
using MediatR;

namespace Journey_of_faith.Application.usecases.auth.commands;


public class ChangePasswordHandler : IRequestHandler<ChangePasswordCommand, Unit>
{
    private readonly IAuthService authService;
    public ChangePasswordHandler(IAuthService authService)
    {
        this.authService = authService;
    }

    public async Task<Unit> Handle(ChangePasswordCommand command, CancellationToken cancellationToken)
    {
        var result = await authService.ChangePassword(command.CurrentPassword, command.NewPassword);
        if(!result)
        {
            throw new UnauthorizationException("Đổi mật khẩu thất bại vui, lòng nhập đúng mật khẩu");
        }

        return Unit.Value;
    }
}