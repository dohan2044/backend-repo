using System.Globalization;
using System.Security.Claims;
using Dapper;
using Journey_of_faith.Application.common.interfaces;
using Journey_of_faith.Application.exceptions;
using Journey_of_faith.Domain.dtos;
using Journey_of_faith.Domain.entities;
using Journey_of_faith.Domain.interfaces;
using Journey_of_faith.Infrastructure.common;
using Journey_of_faith.Infrastructure.context;
using Journey_of_faith.Infrastructure.identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Journey_of_faith.Infrastructure.repositories;

public class RoleRepository : BaseRepository, IRoleRepository
{
    private readonly ApplicationDbContext _dbContext;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;
    public RoleRepository(ApplicationDbContext dbContext,
        IOptions<TableSchemaName> tableSchemaName,
        IDbConnectionFactory dbConnectionFactory,
        RoleManager<ApplicationRole> roleManager,
        UserManager<ApplicationUser> userManager
        )
    : base(dbConnectionFactory, schemaName: tableSchemaName)
    {
        _dbContext = dbContext;
        _roleManager = roleManager;
        _userManager = userManager;
    }


    public async Task<Guid> CreateAsync(Role role, CancellationToken cancellationToken)
    {
        var insert = new ApplicationRole
        {
            Name = role.Name,
            Descriptions = role.Descriptions
        };
        await _dbContext.Roles.AddAsync(insert, cancellationToken);
        await _dbContext.SaveChangesAsync();
        return role.Id;
    }

    public async Task<bool> AddPermissionForRole(string roleName, List<string> permissions)
    {
        var roleExists = await _roleManager.FindByNameAsync(roleName);
        if (roleExists == null)
        {
            throw new BadRequestException("Tên vai trò không tồn tại");
        }
        // lấy ra các claim(quyền cuẩ vai trò);
        var existingClaims = await _roleManager.GetClaimsAsync(roleExists);

        // lấy ra các giá trị trị cúa claim
        var existingPermissionValues = existingClaims
            .Where(x => x.Type == "Permission")
            .Select(x => x.Value)
            .ToHashSet();
        foreach (var claim in existingClaims.Where(x => x.Type == "Permission"))
        {
            await _roleManager.RemoveClaimAsync(roleExists, claim);
        }
        foreach (string permission in permissions)
        {

            var result = await _roleManager.AddClaimAsync(roleExists, new Claim("Permission", permission));

            if (!result.Succeeded)
            {
                throw new BadRequestException("Không thể thêm quyền cho Vai trò");
            }

        }
        ;
        return true;
    }

    public async Task<bool> DeleteRoleAsync(string roleName)
    {
        var role = await _roleManager.FindByNameAsync(roleName);
        if (role == null)
        {
            throw new NotFoundException($"Role '{roleName}' invalid.");
        }

        var userRoleExists = await _userManager.GetUsersInRoleAsync(roleName);
        if (userRoleExists.Any())
        {
            throw new BadRequestException($"Don't delete: {roleName} because {userRoleExists.Count} users used this role.");
        }
        var result = await _roleManager.DeleteAsync(role);
        if (!result.Succeeded)
        {
            var error = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new BadRequestException($"Don't delete {roleName}: {error}");
        }

        return result.Succeeded;
    }
    public async Task<bool> RemoveUserFromRole(Guid userId, string roleName)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user is null)
            {
                throw new NotFoundException("Người dùng không hợp lệ");
            }
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role is null)
            {
                throw new NotFoundException("Vai trò không hợp lệ");
            }


            IdentityResult remove = await _userManager.RemoveFromRoleAsync(user, role.Name);
            if (!remove.Succeeded)
            {
                throw new BadRequestException(string.Join(", ", remove.Errors.Select(e => e.Description)));
            }
            return remove.Succeeded;
        }
        catch (Exception)
        {
            throw;
        }
    }


    public async Task<bool> UpdateRoleAsync(string roleId, Role role)
    {

        var roleExits = await _roleManager.FindByIdAsync(roleId);
        if (roleExits is null)
        {
            throw new NotFoundException($"{role.Name} is not found.");
        }

        roleExits.Name = role.Name ?? roleExits.Name;
        roleExits.Descriptions = role.Descriptions ?? roleExits.Descriptions;

        IdentityResult result = await _roleManager.UpdateAsync(roleExits);
        return result.Succeeded;
    }
}
