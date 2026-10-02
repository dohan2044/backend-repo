using Journey_of_faith.Application.common.interfaces;
using Journey_of_faith.Application.exceptions;
using Journey_of_faith.Infrastructure.context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace Journey_of_faith.Infrastructure.identity.services
{
    // public record User (string username, string PasswordHash, string Name);
    public class IdentityService : IIdentityService
    {
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> roleManager;
        private readonly ApplicationDbContext _context;
        
        public IdentityService(UserManager<ApplicationUser> userManager, 
            RoleManager<ApplicationRole> roleManager, 
            ApplicationDbContext applicationDbContext,
            IMapper mapper)
        {
            _userManager = userManager; 
            this.roleManager = roleManager;
            _context = applicationDbContext;
            _mapper = mapper;
        }

        public async Task<bool> CreateAsync(Domain.entities.User input, string? roleName)
        {
            var user = new ApplicationUser
            {
                UserName = input.Username,
                Email = input.Email,
            };

            var result = await _userManager.CreateAsync(user, input.Password);
            if(!result.Succeeded)
            {
                foreach(var error in result.Errors)
                {
                    throw new UnprocessableEntityException(error.Description);
                }
            }

            string role =null;
            if(!string.IsNullOrEmpty(roleName)) {
                role = roleName;
            } else {
                role = "user";
            }
            if(!await roleManager.RoleExistsAsync(role))
            {
                var newRole = new ApplicationRole
                {
                    Name = role
                };
                await roleManager.CreateAsync(newRole);
            }
            await _userManager.AddToRoleAsync(user, role);
            return true;
        }

        public async Task<bool> ExistsEmail(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null)
                return false;
            return true;
        }

        public async Task<Journey_of_faith.Domain.entities.User?> GetUserByIdAsync(Guid id)
        {
            var user = await _userManager.Users
                .FirstOrDefaultAsync(e => e.Id == id);

            var userMapp = _mapper.Map<Journey_of_faith.Domain.entities.User>(user);
            return userMapp;
        }


        public async Task<bool> UpdateRoleAsync(Guid userId, string oldRoleName, string newRoleName)
        {
            // lấy ra thông tin người dùng
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user is null)
            {
                throw new NotFoundException("Người dùng không tồn tại");
            }

            // xóa người dùng khỏi vai trò 
            IdentityResult remove = await _userManager.RemoveFromRoleAsync(user, oldRoleName);
            if(!remove.Succeeded)
            {
                throw new BadRequestException(string.Join(",", remove.Errors.Select(e => e.Description)));
            }

            // tìm kiểm xem vai trò mới có hợp lệ hay không 
            var roleExists = await roleManager.FindByNameAsync(newRoleName);
            if(roleExists is null)
            {
                throw new NotFoundException("Vai trò mới không hợp lệ.");
            }

            // thêm vai trò mới vào cho người dùng
            IdentityResult addNew = await _userManager.AddToRoleAsync(user, newRoleName);
            if(!addNew.Succeeded)
            {
                throw new BadRequestException(string.Join(",", addNew.Errors.Select(e => e.Description)));
            }
            return true;
        }


        
    }
}
