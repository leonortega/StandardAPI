﻿using System.Data;
using Microsoft.Data.SqlClient;

namespace StandardAPI.Infraestructure.Persistence
{
    public class DatabaseConnectionFactory
    {
        private readonly string _connectionString;

        public DatabaseConnectionFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IDbConnection CreateConnection() => new SqlConnection(_connectionString);
    }
}
