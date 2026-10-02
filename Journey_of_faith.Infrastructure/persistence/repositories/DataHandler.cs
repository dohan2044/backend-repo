using Dapper;
using Journey_of_faith.Application.common.interfaces;
using Journey_of_faith.Infrastructure.common;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

namespace Journey_of_faith.Infrastructure.repositories;


public abstract class DataHandlerRequest : IDataHandler
{
    protected readonly IDbConnectionFactory _factory;
    protected readonly TableSchemaName _schemaName;
    public DataHandlerRequest(IDbConnectionFactory factory, IOptions<TableSchemaName> _options)
    {
        _factory = factory;
        _schemaName = _options.Value;
    }

    public async Task<IEnumerable<T>> GetAllEntityAsync<T>(string table)
    {
        var connection = _factory.CreateConnection();
        return await connection.QueryAsync<T>($"Select * from [{_schemaName.Schema}].[{table}]");
    }

    public async Task<T?> GetEntityDetailsAsync<T>(int Id, string StoredProcedure)
    {
        var connection = _factory.CreateConnection();
        DynamicParameters param = new DynamicParameters();
        param.Add("Id", Id);

        var data = await connection.QueryFirstOrDefaultAsync<T>(
                StoredProcedure, param, commandType: System.Data.CommandType.StoredProcedure
            );
        return data;
    }

    public async Task<bool> InsertOnlyName(string table, object param)
    {
        string command = $"INSERT INTO [{_schemaName.Schema}].[{table}] (Name, Code, Description) VALUES(@Name, @Code, @Description)";
        if (table == TableQuestion.QuizLevel)
        {
            command = $"INSERT INTO [{_schemaName.Schema}].[{table}] (Name, Code, Score) VALUES(@Name, @Code, @Score)";
        }
        using var connection = _factory.CreateConnection();
        var result = await connection.ExecuteAsync(command, param);

        return result > 0;
    }
    public async Task<int> GetCountDataTable(string tableName)
    {
        await using var connection = (SqlConnection)_factory.CreateConnection();
        return await connection.ExecuteScalarAsync<int>(
            $"Select Count(*) From [{_schemaName.Schema}].[{tableName}]"
        );
    }
}