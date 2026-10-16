using Dapper;
using Journey_of_faith.Application.common.interfaces;
using Journey_of_faith.Application.usecases.charities;
using Journey_of_faith.Infrastructure.common;
using Journey_of_faith.Infrastructure.repositories;
using Microsoft.Extensions.Options;

namespace Journey_of_faith.Infrastructure.persistence.queries;

public sealed class CharityQueries : BaseRepository, ICharityQueries
{
    private const string CharityActivitiesTable = "CharityActivities";
    private const string RegistrationsTable = "CharityRegistrations";

    public CharityQueries(IDbConnectionFactory connectionFactory, IOptions<TableSchemaName> options)
        : base(connectionFactory, options) { }

    public Task<bool> CharityExistsAsync(int charityId, CancellationToken cancellationToken = default)
        => ExistsAsync(
            CharityActivitiesTable,
            "Id = @Id AND ISNULL(IsDeleted, 0) = 0",
            new { Id = charityId },
            cancellationToken);

    public Task<bool> RegistrationExistsAsync(
        int charityId,
        Guid userId,
        CancellationToken cancellationToken = default)
        => ExistsAsync(
            RegistrationsTable,
            "ActivityId = @ActivityId AND UserId = @UserId AND ISNULL(IsDeleted, 0) = 0",
            new { ActivityId = charityId, UserId = userId },
            cancellationToken);

    private Task<bool> ExistsAsync(
        string table,
        string predicate,
        object parameters,
        CancellationToken cancellationToken)
        => QueryAsync(async connection =>
            await connection.ExecuteScalarAsync<int>(new CommandDefinition(
                $"SELECT CASE WHEN EXISTS (SELECT 1 FROM [{_schemaName.Schema}].[{table}] WHERE {predicate}) THEN 1 ELSE 0 END",
                parameters,
                cancellationToken: cancellationToken)) > 0);
}
