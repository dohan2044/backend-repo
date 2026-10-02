using Dapper;
using Journey_of_faith.Application.common.dtos;
using Journey_of_faith.Application.common.interfaces;
using Journey_of_faith.Application.usecases.notifications;
using Journey_of_faith.Infrastructure.common;
using Journey_of_faith.Infrastructure.repositories;
using Microsoft.Extensions.Options;

namespace Journey_of_faith.Infrastructure.persistence.queries;

public sealed class UserDeviceQueries : BaseRepository, IUserDeviceQueries
{
    public UserDeviceQueries(IDbConnectionFactory connectionFactory, IOptions<TableSchemaName> options)
        : base(connectionFactory, options) { }

    public Task<UserDeviceInfoDto?> GetUserWithDevicesAsync(Guid userId, CancellationToken cancellationToken = default)
        => QueryAsync(async connection =>
        {
            var rows = (await connection.QueryAsync<UserDeviceRow>(new CommandDefinition(
                $"""
                SELECT u.Id AS UserId, u.Name, u.UserName, u.Email, u.Avatar,
                       d.Id AS DeviceId, d.Platform AS DeviceName, d.CreatedAt
                FROM [{_schemaName.Schema}].[User] AS u
                LEFT JOIN [{_schemaName.Schema}].[DeviceToken] AS d ON d.UserId = u.Id
                WHERE u.Id = @UserId
                ORDER BY d.CreatedAt DESC
                """,
                new { UserId = userId },
                cancellationToken: cancellationToken))).ToList();

            var user = rows.FirstOrDefault();
            if (user is null)
                return null;

            return new UserDeviceInfoDto
            {
                UserId = user.UserId,
                Name = user.Name ?? string.Empty,
                UserName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                Avatar = user.Avatar,
                Devices = rows
                    .Where(row => row.DeviceId.HasValue)
                    .Select(row => new UserDeviceDto
                    {
                        Id = row.DeviceId!.Value,
                        DeviceName = row.DeviceName,
                CreatedAt = row.CreatedAt ?? default
                    })
                    .ToList()
            };
        });

    public Task<List<UserDeviceTokenDto>> GetAllUserDeviceTokensAsync(CancellationToken cancellationToken = default)
        => QueryAsync(async connection => (await connection.QueryAsync<UserDeviceTokenDto>(new CommandDefinition(
            $"""
            SELECT d.UserId, u.UserName, d.Token AS TokenDevice
            FROM [{_schemaName.Schema}].[DeviceToken] AS d
            INNER JOIN [{_schemaName.Schema}].[User] AS u ON u.Id = d.UserId
            ORDER BY u.UserName, d.CreatedAt DESC
            """,
            cancellationToken: cancellationToken))).ToList());

    private sealed class UserDeviceRow
    {
        public Guid UserId { get; set; }
        public string? Name { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? Avatar { get; set; }
        public int? DeviceId { get; set; }
        public string? DeviceName { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
