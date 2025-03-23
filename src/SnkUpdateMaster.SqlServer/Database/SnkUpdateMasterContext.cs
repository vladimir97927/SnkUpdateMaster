using Microsoft.EntityFrameworkCore;
using SnkUpdateMaster.Core.ReleasePublisher;

namespace SnkUpdateMaster.SqlServer.Database
{
    public class SnkUpdateMasterContext(DbContextOptions options) : DbContext(options)
    {
        public DbSet<Release> Releases { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(SnkUpdateMasterContext).Assembly);
        }
    }
}
