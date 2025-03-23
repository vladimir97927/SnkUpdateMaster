using Microsoft.Data.SqlClient;
using System.Data;

namespace SnkUpdateMaster.SqlServer.Configuration.Data
{
    public class SqlConnectionFactory : ISqlConnectionFactory, IDisposable
    {
        private readonly string _connectionString;

        private IDbConnection? _connection;

        public SqlConnectionFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void Dispose()
        {
            if (_connection != null && _connection.State == ConnectionState.Open)
            {
                _connection.Dispose();
            }
        }

        public IDbConnection GetOpenConnection()
        {
            if (_connection == null || _connection.State != ConnectionState.Open)
            {
                _connection = new SqlConnection(_connectionString);
                _connection.Open();
            }

            return _connection;
        }
    }
}
