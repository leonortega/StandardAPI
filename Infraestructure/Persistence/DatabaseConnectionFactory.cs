using System.Data;
using Npgsql;

namespace StandardAPI.Infraestructure.Persistence
{
    public class DatabaseConnectionFactory
    {
        private readonly string _connectionString;

        public DatabaseConnectionFactory(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        public IDbConnection CreateConnection() => new NpgsqlConnection(_connectionString);
    }
}
