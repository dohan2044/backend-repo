using Journey_of_faith.Application.common.interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Journey_of_faith.Application.usecases.auth.queries
{
    public class UserLoginHandler : IRequestHandler<UserLoginQuery, UserLoginResponse>
    {
        private readonly IAuthService _auth;
        public UserLoginHandler(IAuthService authService)
        {
            _auth = authService;
        }

        public async Task<UserLoginResponse> Handle(UserLoginQuery query, CancellationToken tokens)
        {
            var (status, token, refreshToken, expiry) = await _auth.Login(query.Email, query.Password);
            return new UserLoginResponse(status, token, refreshToken, expiry);
        }
    }
}
