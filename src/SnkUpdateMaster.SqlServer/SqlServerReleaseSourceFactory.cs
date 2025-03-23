using Microsoft.EntityFrameworkCore;
using SnkUpdateMaster.Core.ReleasePublisher;
using SnkUpdateMaster.SqlServer.Database;

namespace SnkUpdateMaster.SqlServer
{
    public class SqlServerReleaseSourceFactory : IReleaseSourceFactory
    {
        private readonly SnkUpdateMasterContext _context;

        public SqlServerReleaseSourceFactory(string connectionString)
        {
            var dbContextOptionsBuilder = new DbContextOptionsBuilder<SnkUpdateMasterContext>();
            dbContextOptionsBuilder.UseSqlServer(connectionString);
            _context = new SnkUpdateMasterContext(dbContextOptionsBuilder.Options);
        }

        public IReleaseSource Create()
        {
            return new SqlServerReleaseSource(_context);
        }
    }
}
