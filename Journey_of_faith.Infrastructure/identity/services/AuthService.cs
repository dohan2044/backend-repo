using Journey_of_faith.Application.common.interfaces;
using Journey_of_faith.Application.exceptions;
using Journey_of_faith.Domain.entities;
using Journey_of_faith.Infrastructure.context;
using Journey_of_faith.Infrastructure.services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using System.Text;

namespace Journey_of_faith.Infrastructure.identity.services
{
    public class AuthService : IAuthService
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly TokenService _tokenService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;
        private readonly IConfiguration configuration;
        private readonly IEmailService emailService;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ILogger<AuthService> _logger;
        public AuthService(TokenService tokenService, UserManager<ApplicationUser> userManager,
            ApplicationDbContext context, ICurrentUserService currentUserService, IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor, IEmailService emailService, RoleManager<ApplicationRole> roleManager,
            ILogger<AuthService> logger)
        {
            _tokenService = tokenService;
            _userManager = userManager;
            _context = context;
            _currentUser = currentUserService;
            _roleManager = roleManager;
            this.configuration = configuration;
            this.httpContextAccessor = httpContextAccessor;
            this.emailService = emailService;
            _logger = logger;
        }
        public async Task<LoginUserResponse> Login(string email, string passwrod)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null)
            {
                _logger.LogWarning("Login rejected because the account was not found");
                throw new UnauthorizationException("Tài khoản hoặc mật khẩu không chính xác");
            }            
            var result = await _userManager.CheckPasswordAsync(user, passwrod);
            if (!result)
            {
                _logger.LogWarning("Login rejected for user {UserId} because the password was invalid", user.Id);
                throw new UnauthorizationException("Tài khoản hoặc mật khẩu không chính xác");
            }

            var expiredToken = await _context.RefreshTokens.Where(e => e.UserId == user.Id).ToListAsync();
            if(expiredToken.Any())
            {
                _context.RefreshTokens.RemoveRange(expiredToken);
            }
            List<string> claims = new List<string>();
            var roles = await _userManager.GetRolesAsync(user);
            foreach(var role in roles)
            {
                var roleExists = await _roleManager.FindByNameAsync(role);
                var claimsRole = await _roleManager.GetClaimsAsync(roleExists);
                claims.AddRange(claimsRole.Where(e => string.Equals(e.Type, "Permission")).Select(e => e.Value));
            }
            var token = _tokenService.GenerateToken(user, roles.ToList(), claims);
            var refreshToken = _tokenService.CreateRefreshToken(user.Id);

            await _context.RefreshTokens.AddAsync(refreshToken);
            var active = new UserActive
            {
                ApplicationUserId = user.Id,
                Status = true,
                ActiveLocation = "ha",
                Timespan = DateTime.UtcNow,
            };
            await _context.UserActive.AddAsync(active);
            await _context.SaveChangesAsync();
            _logger.LogInformation("User {UserId} logged in successfully", user.Id);
            return new LoginUserResponse(status: true, token: token, refreshToken: refreshToken.Token, expiry: configuration.GetValue<int>("Token:Expiry"));
        }

        public async Task<LoginUserResponse> RefreshToken(string refreshToken)
        {
            var refresh = await _context.RefreshTokens
                .FirstOrDefaultAsync(t => t.Token == refreshToken);

            if (refresh is null || refresh.ExpiresOnUtc < DateTime.UtcNow)
            {
                _logger.LogWarning("Refresh token request was rejected because the token was invalid or expired");
                throw new UnauthorizationException("Refresh token đã hết hạn hoặc không hợp lệ.");
            }

            var user = await _userManager.FindByIdAsync(refresh.UserId.ToString());
            var roles = await _userManager.GetRolesAsync(user);
            var expiry = configuration.GetValue<int>("Token:Expiry");
            List<string> claims = new List<string>();
            foreach(var role in roles)
            {
                var roleExists = await _roleManager.FindByNameAsync(role);
                var claimsRole = await _roleManager.GetClaimsAsync(roleExists);
                claims.AddRange(claimsRole.Where(e => string.Equals(e.Type, "Permission")).Select(e => e.Value));
            }
            var newToken = _tokenService.GenerateToken(user, roles.ToList(), claims);
            var newRefreshToken = _tokenService.CreateRefreshToken(user.Id);

            _context.RefreshTokens.Remove(refresh);
            await _context.RefreshTokens.AddAsync(newRefreshToken);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Refreshed authentication tokens for user {UserId}", user.Id);
            return new LoginUserResponse(true, token: newToken, refreshToken: newRefreshToken.Token, expiry);
        }


        public async Task<bool> ChangePassword(string currentPassword, string newPassword)
        {
            if (string.IsNullOrEmpty(currentPassword))
            {
                throw new ArgumentException(nameof(currentPassword));
            }

            if (string.IsNullOrEmpty(newPassword))
            {
                throw new ArgumentNullException(nameof(newPassword));
            }
            var userId = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new UnauthorizationException("Vui long dang nhap");
            }

            var isValid = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
            if (!isValid.Succeeded)
            {
                _logger.LogWarning("Password change failed for user {UserId}", user.Id);
                return false;
            }

            _logger.LogInformation("Password changed successfully for user {UserId}", user.Id);
            return true;
        }

        public async Task<bool> ResetPassword(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null)
            {
                _logger.LogWarning("Password reset requested for an account that does not exist");
                return false;
            }

            // generate
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            string newPassword = PasswordGenerator.GenerateRandomPassword(14);

            IdentityResult result = await _userManager.ResetPasswordAsync(user, token, newPassword);
            if (result.Succeeded)
            {
                // Tiêu đề Email
                string subject = "[Hành Trình Đức Tin] Cấp lại mật khẩu tài khoản mới";

                // Thân bài Email dạng HTML (nhìn chữ mật khẩu sẽ to, rõ ràng, dễ copy)
                string body = $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #dee2e6; border-radius: 5px;'>
                <h2 style='color: #007bff; text-align: center;'>Khôi Phục Mật Khẩu Thành Công</h2>
                <p>Chào bạn,</p>
                <p>Hệ thống đã nhận được yêu cầu cấp lại mật khẩu cho tài khoản <strong>{email}</strong> trên ứng dụng Journey of Faith.</p>
                            <div style='background-color: #f8f9fa; padding: 15px; text-align: center; border-radius: 4px; margin: 20px 0;'>
                    <p style='margin: 0; color: #6c757d;'>Mật khẩu đăng nhập mới của bạn là:</p>
                    <p style='margin: 10px 0 0 0; font-size: 22px; font-weight: bold; color: #dc3545; letter-spacing: 1px;'>{newPassword}</p>
                </div>

                <p style='color: #dc3545;'>* Lưu ý: Để bảo mật, vui lòng đăng nhập vào hệ thống và tiến hành đổi lại mật khẩu ngay sau khi nhận được email này.</p>
                <hr style='border: none; border-top: 1px solid #dee2e6; margin: 20px 0;'>
                <p style='font-size: 12px; color: #6c757d; text-align: center;'>Đây là email tự động, vui lòng không phản hồi lại email này.</p>
                </div>";
                await emailService.SendEmailAsync(email, subject, body);

                _logger.LogInformation("Password reset completed for user {UserId}", user.Id);
                return true;
            }

            _logger.LogWarning("Password reset failed for user {UserId}", user.Id);
            return false;
        }

        public async Task<User> GetMe()
        {
            var userId = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier ?? "sub");
            if(userId == null)
            {
                throw new BadRequestException("Nguoi dung khong hop le");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new NotFoundException("Khong tim thay nguoi dung");
            }

            var userProvince = await _context
                .Users.AsNoTracking().Include(e => e.Province)
                .Select(e => new
                {
                    Id = e.Id,
                    Name = e.Province.Name
                })
                .FirstOrDefaultAsync(e => e.Id == Guid.Parse(userId));
            var userRole = await _userManager.GetRolesAsync(user);
            return new User(
                user.Email,
                user.UserName,
                user.Avatar,
                userRole.ToList(),
                userProvince.Name,
                user.Score,
                user.DayStreak
            );
        }
    }
}
