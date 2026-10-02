using Journey_of_faith.Application.common.dtos;
using Journey_of_faith.Domain.dtos;

namespace Journey_of_faith.Application.usecases.auth;

public interface IRoleQueries
{
    Task<PagedResult<RoleViewDto>> GetRolesAsync(int page, int pageSize, string? search, CancellationToken cancellationToken = default);
    Task<Dictionary<string, int>> GetTotalUsersByRoleAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RolePermissionDto>> GetPermissionsAsync(CancellationToken cancellationToken = default);
    Task<bool> NameExistsAsync(string name, CancellationToken cancellationToken = default);
}
