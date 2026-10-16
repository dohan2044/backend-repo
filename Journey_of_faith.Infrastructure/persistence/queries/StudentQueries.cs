
using Dapper;
using Journey_of_faith.Application.common.interfaces;
using Journey_of_faith.Application.usecases.students;
using Journey_of_faith.Infrastructure.common;
using Journey_of_faith.Infrastructure.repositories;
using Microsoft.Extensions.Options;

namespace Journey_of_faith.Infrastructure.persistence.queries;


public class StudentQueries : BaseRepository, IStudentQueries
{
    public StudentQueries(IDbConnectionFactory factory, IOptions<TableSchemaName> schemaName) 
    : base(factory, schemaName)
    {
    }

    public Task<bool> ExistsGroupNameAsync(string groupName, CancellationToken cancellationToken = default)
    {
        return QueryAsync(async connection =>
        {
            var command = new CommandDefinition(
                $@"SELECT COUNT(*) FROM [{_schemaName.Schema}].[{StudentNameTableQueries.StudentGroupTable}] WHERE Name = @groupName AND ISNULL(IsDeleted, 0) = 0",
                new { groupName },
                cancellationToken: cancellationToken);
            var count = await connection.ExecuteScalarAsync<int>(command);
            return count > 0;
        });
    }

    public async Task<bool> ExistsGroupIdAsync(int groupId, CancellationToken cancellationToken = default)
    {
        return await QueryAsync(async connection =>
        {
           var command = new CommandDefinition(
            @$"Select count(id) from {_schemaName.Schema}.{StudentNameTableQueries.StudentGroupTable} where id = @groupId",
            new {groupId},
            cancellationToken: cancellationToken
           );

           return await connection.ExecuteScalarAsync<int>(command) > 0;
        });
    }
}


public static class StudentNameTableQueries
{
    public const string StudentGroupTable = "StudentGroups";
}