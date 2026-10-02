
using Asp.Versioning;
using Azure.Core;
using Journey_of_faith.Api.Attributes;
using Journey_of_faith.Api.authorization;
using Journey_of_faith.Api.dtos;
using Journey_of_faith.Application.usecases.auth.commands;
using Journey_of_faith.Application.usecases.auth.queries;
using Journey_of_faith.Application.common.dtos;
using Journey_of_faith.Domain.dtos;
using Journey_of_faith.Domain.entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Journey_of_faith.Api.Controllers
{
    [ApiVersion(1)]
    [ApiController]
    [Route("api/v{version:apiVersion}/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _me;
        public AuthController(IMediator me)
        {
            _me = me;
        }
        [MapToApiVersion(1)]
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] UserLoginQuery query)
        {
            var login = await _me.Send(query);
            return Ok(new ApiResponse<UserLoginResponse>
            {
                Message = "Đăng nhập thành công",
                Data = login
            });
        }
        [MapToApiVersion(1)]
        [AllowAnonymous]
        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken([FromBody] CreateRefreshTokenCommand command)
        {
            var refresh = await _me.Send(command);
            return Ok(refresh);
        }
        [MapToApiVersion(1)]
        [Authorize]
        [HttpPatch("change")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordCommand command)
        {
            await _me.Send(command);
            return NoContent();
        }
        [MapToApiVersion(1)]
        [HasPermission(Permissions.Roles.VIEW)]
        [HttpGet("roles")]
        public async Task<IActionResult> GetRoles([FromQuery] int page, [FromQuery] int pageSize, [FromQuery] string? search)
        {
            var roles = await _me.Send(new GetRolesQuery { Page = page, PageSize = pageSize, Search = search });
            return Ok(new ApiResponse<PagedResult<RoleViewDto>>
            {
                Data = roles,
                Message = "Lấy dữ liệu thành công"
            });
        }
        [MapToApiVersion(1)]
        [HasPermission(Permissions.Roles.VIEW)]
        [HttpGet("roles/users-role")]
        public async Task<IActionResult> GetTotalRole()
        {
            var total = await _me.Send(new GetTotalUserPerRoleQuery());
            return Ok(new ApiResponse<Dictionary<string, int>>
            {
                Message = "Lấy dữ liệu thành công",
                Data = total
            });
        }
        [MapToApiVersion(1)]
        [HasPermission(Permissions.Roles.CREATE)]
        [HttpPost("roles")]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleCommand command)
        {
            var roleId = await _me.Send(command);
            return StatusCode(StatusCodes.Status201Created, new ApiResponse<Guid>
            {
                Message = "Tạo vai trò thành công.",
                Data = roleId
            });
        }
        [MapToApiVersion(1)]
        [HasPermission(Permissions.Roles.CREATE)]
        [HttpPost("roles/add-permission")]
        public async Task<IActionResult> AddPermission([FromBody] AddPermissionCommand command)
        {
            var isSuccess = await _me.Send(command);
            return Ok(new ApiResponse<bool>
            {
                Message = "Thêm vai trò thành công",
                Data = isSuccess
            });
        }
        [MapToApiVersion(1)]
        [HasPermission(Permissions.Roles.VIEW)]
        [HttpGet("roles/get-permissions")]
        public async Task<IActionResult> GetPermission()
        {
            var result = await _me.Send(new GetPermissionsQuery());
            return Ok(new ApiResponse<IReadOnlyList<RolePermissionDto>>
            {
                Message = "Lay quyen thanh cong",
                Data = result
            });
        }
        [MapToApiVersion(1)]
        [HasPermission(Permissions.Roles.DELETE)]
        [HttpDelete("roles")]
        public async Task<IActionResult> Delete([FromQuery] string Name)
        {
            await _me.Send(new DeleteRoleCommand { RoleName = Name });
            return NoContent();
        }
        [MapToApiVersion(1)]
        [HasPermission(Permissions.Roles.DELETE)]
        [HttpDelete("roles/remove-user")]
        public async Task<IActionResult> RemoveUserRole([FromBody] DeleteUserFromRoleCommand command)
        {
            await _me.Send(new DeleteUserFromRoleCommand { userId = command.userId, RoleName = command.RoleName });
            return NoContent();
        }

        [MapToApiVersion(1)]
        [HasPermission(Permissions.Roles.EDIT)]
        [HttpPut("roles")]
        public async Task<IActionResult> UpdateRole([FromBody] UpDateRoleCommand command)
        {
            await _me.Send(command);
            return NoContent();
        }

        [MapToApiVersion(1)]
        [AllowAnonymous]
        [HttpPost("reset")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand command)
        {
            var result = await _me.Send(command);
            return NoContent();
        } 
        // ==============================
        // 🚀 GOOGLE LOGIN ENDPOINT
        // ==============================
        [HttpPost("google-login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [MapToApiVersion(1)]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequest request)
        {
            try
            {
                // Log request nhận được (ẩn bớt token)
                Console.WriteLine($"🟢 GoogleLogin endpoint called");
                Console.WriteLine($"🟢 Token length: {request.IdToken?.Length ?? 0}");

                var result = await _me.Send(new GoogleLoginCommand { IdToken = request.IdToken });

                return Ok(new ApiResponse<LoginResponse>
                {
                    Message = "Đăng nhập Google thành công",
                    Data = result
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"❌ Unauthorized: {ex.Message}");
                return Unauthorized(new ApiResponse<string>
                {
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Internal error: {ex.Message}");
                Console.WriteLine($"❌ Stack: {ex.StackTrace}");
                return StatusCode(500, new ApiResponse<string>
                {
                    Message = $"Lỗi đăng nhập Google: {ex.Message}"
                });
            }
        }
    }
}
