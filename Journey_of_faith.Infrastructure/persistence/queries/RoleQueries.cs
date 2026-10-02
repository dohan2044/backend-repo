using Dapper;
using Journey_of_faith.Application.common.dtos;
using Journey_of_faith.Application.common.interfaces;
using Journey_of_faith.Application.usecases.auth;
using Journey_of_faith.Domain.dtos;
using Journey_of_faith.Infrastructure.common;
using Journey_of_faith.Infrastructure.context;
using Journey_of_faith.Infrastructure.repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Journey_of_faith.Infrastructure.persistence.queries;

public sealed class RoleQueries : BaseRepository, IRoleQueries
{
    private readonly ApplicationDbContext _dbContext;

    public RoleQueries(
        ApplicationDbContext dbContext,
        IDbConnectionFactory connectionFactory,
        IOptions<TableSchemaName> options)
        : base(connectionFactory, options)
    {
        _dbContext = dbContext;
    }

    public async Task<PagedResult<RoleViewDto>> GetRolesAsync(
        int page,
        int pageSize,
        string? search,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Max(pageSize, 1);
        var query = _dbContext.Roles.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var keyword = search.Trim();
            query = query.Where(role => role.Name != null && role.Name.Contains(keyword));
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var data = await query
            .OrderByDescending(role => role.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(role => new RoleViewDto
            {
                Id = role.Id,
                Name = role.Name ?? string.Empty,
                Description = role.Descriptions
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<RoleViewDto>
        {
            TotalCount = totalCount,
            Data = data,
            Page = page,
            PageSize = pageSize
        };
    }

    public Task<Dictionary<string, int>> GetTotalUsersByRoleAsync(CancellationToken cancellationToken = default)
        => QueryAsync(async connection =>
        {
            var rows = await connection.QueryAsync<(string Name, int TotalUserPerRole)>(new CommandDefinition(
                $"""
                SELECT role.Name, COUNT(*) AS TotalUserPerRole
                FROM [{_schemaName.Schema}].[User] userAccount
                INNER JOIN [{_schemaName.Schema}].[AspNetUserRoles] userRole ON userAccount.Id = userRole.UserId
                INNER JOIN [{_schemaName.Schema}].[AspNetRoles] role ON userRole.RoleId = role.Id
                GROUP BY role.Name
                """,
                cancellationToken: cancellationToken));
            return rows.ToDictionary(row => row.Name, row => row.TotalUserPerRole);
        });

    public Task<IReadOnlyList<RolePermissionDto>> GetPermissionsAsync(CancellationToken cancellationToken = default)
        => QueryAsync<IReadOnlyList<RolePermissionDto>>(async connection =>
        {
            var rows = await connection.QueryAsync<RolePermissionDto>(new CommandDefinition(
                $"""
                SELECT role.Id AS RoleId, role.Name AS RoleName, claim.ClaimType, claim.ClaimValue
                FROM [{_schemaName.Schema}].[AspNetRoles] role
                INNER JOIN [{_schemaName.Schema}].[AspNetRoleClaims] claim ON role.Id = claim.RoleId
                """,
                cancellationToken: cancellationToken));
            return rows.ToList();
        });

    public Task<bool> NameExistsAsync(string name, CancellationToken cancellationToken = default)
        => _dbContext.Roles.AsNoTracking().AnyAsync(role => role.Name == name, cancellationToken);
}
