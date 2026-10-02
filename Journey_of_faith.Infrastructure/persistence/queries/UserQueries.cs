using Dapper;
using Journey_of_faith.Application.common.dtos;
using Journey_of_faith.Application.common.interfaces;
using Journey_of_faith.Application.usecases.users;
using Journey_of_faith.Domain.dtos;
using Journey_of_faith.Infrastructure.common;
using Journey_of_faith.Infrastructure.repositories;
using Microsoft.Extensions.Options;

namespace Journey_of_faith.Infrastructure.persistence.queries;

public sealed class UserQueries : BaseRepository, IUserQueries
{
    public UserQueries(IDbConnectionFactory connectionFactory, IOptions<TableSchemaName> options)
        : base(connectionFactory, options) { }

    public Task<PagedResult<UserResponseDto>> GetUsersAsync(
        int page,
        int pageSize,
        string? search,
        CancellationToken cancellationToken = default)
        => QueryAsync(async connection =>
        {
            page = Math.Max(page, 1);
            pageSize = Math.Max(pageSize, 1);
            var offset = (page - 1) * pageSize;
            var keyword = string.IsNullOrWhiteSpace(search) ? null : search.Trim();
            var parameters = new { Offset = offset, PageSize = pageSize, Keyword = keyword };

            var data = (await connection.QueryAsync<UserResponseDto>(new CommandDefinition(
                $"""
                SELECT userAccount.Id, userAccount.UserName, userAccount.Email,
                       role.Name AS Role, userAccount.Avatar, userAccount.IsDeleted
                FROM [{_schemaName.Schema}].[User] userAccount
                LEFT JOIN [{_schemaName.Schema}].[AspNetUserRoles] userRole ON userAccount.Id = userRole.UserId
                LEFT JOIN [{_schemaName.Schema}].[AspNetRoles] role ON userRole.RoleId = role.Id
                WHERE ISNULL(userAccount.IsDeleted, 0) = 0
                  AND (@Keyword IS NULL OR userAccount.UserName LIKE '%' + @Keyword + '%'
                       OR userAccount.Email LIKE '%' + @Keyword + '%')
                ORDER BY userAccount.Id
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY
                """,
                parameters,
                cancellationToken: cancellationToken))).ToList();

            var totalCount = await connection.ExecuteScalarAsync<int>(new CommandDefinition(
                $"""
                SELECT COUNT(*) FROM [{_schemaName.Schema}].[User]
                WHERE ISNULL(IsDeleted, 0) = 0
                  AND (@Keyword IS NULL OR UserName LIKE '%' + @Keyword + '%' OR Email LIKE '%' + @Keyword + '%')
                """,
                new { Keyword = keyword },
                cancellationToken: cancellationToken));

            return new PagedResult<UserResponseDto>
            {
                Data = data,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        });

    public Task<UserResponseDto?> GetUserAsync(Guid id, CancellationToken cancellationToken = default)
        => QueryAsync(async connection =>
            await connection.QueryFirstOrDefaultAsync<UserResponseDto>(new CommandDefinition(
                $"""
                SELECT userAccount.Id, userAccount.UserName, userAccount.Email,
                       role.Name AS Role, userAccount.Avatar, userAccount.IsDeleted,
                       userAccount.Score, userAccount.DayStreak
                FROM [{_schemaName.Schema}].[User] userAccount
                LEFT JOIN [{_schemaName.Schema}].[AspNetUserRoles] userRole ON userAccount.Id = userRole.UserId
                LEFT JOIN [{_schemaName.Schema}].[AspNetRoles] role ON userRole.RoleId = role.Id
                WHERE userAccount.Id = @Id
                """,
                new { Id = id },
                cancellationToken: cancellationToken)));
}
