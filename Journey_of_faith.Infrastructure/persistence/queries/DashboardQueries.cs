using Dapper;
using Journey_of_faith.Application.common.dtos;
using Journey_of_faith.Application.common.interfaces;
using Journey_of_faith.Application.usecases.dashboard;
using Journey_of_faith.Infrastructure.common;
using Journey_of_faith.Infrastructure.repositories;
using Microsoft.Extensions.Options;

namespace Journey_of_faith.Infrastructure.persistence.queries;

public sealed class DashboardQueries : BaseRepository, IDashboardQueries
{
    public DashboardQueries(IDbConnectionFactory connectionFactory, IOptions<TableSchemaName> options)
        : base(connectionFactory, options) { }

    public Task<DashboardInfoDto> GetDashboardInfoAsync(CancellationToken cancellationToken = default)
        => QueryAsync(async connection =>
        {
            using var multi = await connection.QueryMultipleAsync(new CommandDefinition(
                "spGetDashboardInfo",
                commandType: System.Data.CommandType.StoredProcedure,
                cancellationToken: cancellationToken));

            return new DashboardInfoDto
            {
                UserCount = await multi.ReadFirstOrDefaultAsync<int>(),
                ChurchCount = await multi.ReadFirstOrDefaultAsync<int>(),
                QuestionCount = await multi.ReadFirstOrDefaultAsync<int>(),
                EventCount = await multi.ReadFirstOrDefaultAsync<int>(),
                RoleCount = await multi.ReadFirstOrDefaultAsync<int>(),
                AccessCount = 1000
            };
        });
}
