
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

    public Task<bool> ExistsGroupNameAsync(string groupName)
    {
        return QueryAsync(async connection =>
        {
            var query = $@"SELECT COUNT(*) FROM {_schemaName.Schema}.{StudentNameTableQueries.StudentGroupTable} WHERE Name = @groupName";
            var count = await connection.ExecuteScalarAsync<int>(query, new { groupName });
            return count > 0;
        });
    }
}


public static class StudentNameTableQueries
{
    public const string StudentGroupTable = "StudentGroups";
}