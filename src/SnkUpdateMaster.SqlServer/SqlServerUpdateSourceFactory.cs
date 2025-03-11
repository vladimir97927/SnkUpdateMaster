using Microsoft.EntityFrameworkCore;
using SnkUpdateMaster.Core.UpdateSource;
using SnkUpdateMaster.SqlServer.Database;

namespace SnkUpdateMaster.SqlServer
{
    public class SqlServerUpdateSourceFactory(string connectionString) : IUpdateSourceFactory
    {
        private readonly string _connectionString = connectionString;

        public IUpdateSource Create()
        {
            var dbContextOptionsBuilder = new DbContextOptionsBuilder<SnkUpdateMasterContext>();
            dbContextOptionsBuilder.UseSqlServer(_connectionString);
            var context = new SnkUpdateMasterContext(dbContextOptionsBuilder.Options);

            return new SqlServerUpdateSource(context);
        }
    }
}
