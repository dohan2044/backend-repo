using Journey_of_faith.Application.common.interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Journey_of_faith.Application.usecases.auth.commands
{
    public class CreateRefreshTokenHandler : IRequestHandler<CreateRefreshTokenCommand, LoginUserResponse>
    {
        private readonly IAuthService _authService;
        public CreateRefreshTokenHandler(IAuthService authService)
        {
            _authService = authService; 
        }

        public async Task<LoginUserResponse> Handle(CreateRefreshTokenCommand command, CancellationToken token)
        {
            var refresh = await _authService.RefreshToken(command.Token);
            return new LoginUserResponse(status: refresh.status, token: refresh.token, refreshToken: refresh.refreshToken, expiry: refresh.expiry);
        }
    }
}
