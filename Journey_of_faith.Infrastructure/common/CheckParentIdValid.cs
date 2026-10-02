using Dapper;
using Journey_of_faith.Application.common.interfaces;
using Journey_of_faith.Domain.interfaces;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace Journey_of_faith.Infrastructure.common
{
    public class CheckParentIdValid(IQuestionRepository question, IOptions<TableSchemaName> options, IDbConnectionFactory factory)
    {
        private readonly IQuestionRepository questionRepository = question;
        private readonly TableSchemaName _name = options.Value;
        private readonly IDbConnectionFactory _factory = factory;
        public async Task<bool> CheckValidId(int id, string table)
        {
            var command = $@"IF EXISTS (SELECT 1 FROM [{_name.Schema}].[{table}] where Id = @Id)
                                SELECT 1 ELSE SELECT 0";
            using var connection = _factory.CreateConnection();
            var result = await connection.ExecuteScalarAsync<int>(command, new { Id = id });
            return result == 1;
        }
    }
}
