using Journey_of_faith.Application.usecases.auth.queries;
using Journey_of_faith.Domain.entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Journey_of_faith.Application.common.interfaces
{
    public record LoginUserRequest(string username, string password);
    public record LoginUserResponse(bool status, string token, string refreshToken, int expiry);
    public interface IAuthService
    {
        Task<LoginUserResponse> Login(string username, string passwrod);
        Task<LoginUserResponse> RefreshToken(string refreshToken);
        Task<bool> ChangePassword(string currentPassword, string newPassword);
        Task<bool> ResetPassword(string email);
        Task<User> GetMe();
    }
}
