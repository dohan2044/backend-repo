using Dapper;
using Journey_of_faith.Application.common.interfaces;
using Journey_of_faith.Application.usecases.events;
using Journey_of_faith.Infrastructure.common;
using Journey_of_faith.Infrastructure.repositories;
using Microsoft.Extensions.Options;

namespace Journey_of_faith.Infrastructure.persistence.queries;

public sealed class StudentEventQueries : BaseRepository, IStudentEventQueries
{
    private const string StudentEventsTable = "StudentEvents";
    private const string RegistrationsTable = "StudentEventRegistrations";

    public StudentEventQueries(IDbConnectionFactory connectionFactory, IOptions<TableSchemaName> options)
        : base(connectionFactory, options) { }

    public Task<bool> StudentEventExistsAsync(int studentEventId, CancellationToken cancellationToken = default)
        => ExistsAsync(
            StudentEventsTable,
            "Id = @Id AND ISNULL(IsDeleted, 0) = 0",
            new { Id = studentEventId },
            cancellationToken);

    public Task<bool> RegistrationExistsAsync(
        int studentEventId,
        Guid userId,
        CancellationToken cancellationToken = default)
        => ExistsAsync(
            RegistrationsTable,
            "EventId = @EventId AND UserId = @UserId AND ISNULL(IsDeleted, 0) = 0",
            new { EventId = studentEventId, UserId = userId },
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
