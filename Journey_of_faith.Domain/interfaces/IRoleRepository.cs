using Journey_of_faith.Domain.entities;

namespace Journey_of_faith.Domain.interfaces;


public interface IRoleRepository
{
    Task<Guid> CreateAsync(Role role, CancellationToken cancellationToken);
    Task<bool> AddPermissionForRole(string roleName, List<string> permissions);

    Task<bool> DeleteRoleAsync(string roleName);
    Task<bool> RemoveUserFromRole(Guid userId, string roleName);
    Task<bool> UpdateRoleAsync(string roleId, Role role);
}
