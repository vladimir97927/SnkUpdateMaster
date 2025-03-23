using SnkUpdateMaster.Core.UpdateSource;
using SnkUpdateMaster.SqlServer.Configuration.Data;

namespace SnkUpdateMaster.SqlServer
{
    public class SqlServerUpdateSourceFactory(string connectionString) : IUpdateSourceFactory
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory = new SqlConnectionFactory(connectionString);

        public IUpdateSource Create()
        {
            return new SqlServerUpdateSource(_sqlConnectionFactory);
        }
    }
}
