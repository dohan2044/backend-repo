using Journey_of_faith.Application.common.interfaces;
using Journey_of_faith.Infrastructure.common;
using Microsoft.AspNetCore.Mvc.TagHelpers.Cache;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Journey_of_faith.Infrastructure.repositories
{
    public class BaseRepository
    {
        protected readonly IDbConnectionFactory _dbConnection;
        protected readonly TableSchemaName _schemaName = new();
        public BaseRepository(IDbConnectionFactory dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public BaseRepository(
            IDbConnectionFactory dbConnection,
            IOptions<TableSchemaName> schemaName)
            : this(dbConnection)
        {
            _schemaName = schemaName.Value;
        }
        protected async Task<TResult> ExecuteAsync<TResult>(Func<IDbConnection, Task<TResult>> query)
        {
            using var connection = _dbConnection.CreateConnection();
            return await query(connection);
        }

        protected async Task<TResult> QueryAsync<TResult>(Func<IDbConnection, Task<TResult>> query)
        {
            using var connection = _dbConnection.CreateConnection();
            return await query(connection);
        }
        protected async Task<Dictionary<TData, TResult>> QueryAsync<TData, TResult>(Func<IDbConnection, Task<Dictionary<TData, TResult>>> query) where TData : notnull
        {
            using var connection = _dbConnection.CreateConnection();
            return await query(connection);
        }
    }
}
