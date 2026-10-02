using Journey_of_faith.Application.common.interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Journey_of_faith.Infrastructure.services
{
    public class DbConnectionFactory : IDbConnectionFactory
    {
        private readonly IConfiguration _config;
        private readonly string _connectionString;
        public DbConnectionFactory(IConfiguration config)
        {
            _config = config;
            _connectionString = config.GetConnectionString("Connection") ?? string.Empty;
        }

        public IDbConnection CreateConnection() {
            return new SqlConnection(_connectionString);
        } 
    }
}
