using SnkUpdateMaster.Core.ReleasePublisher;
using SnkUpdateMaster.SqlServer.Database;

namespace SnkUpdateMaster.SqlServer.Configuration
{
    public static class ReleaseManagerBuilderSqlServerExtensions
    {
        public static ReleaseManagerBuilder WithSqlServerReleaseSource(
            this ReleaseManagerBuilder builder,
            string connectionString)
        {
            var factory = new SqlServerReleaseSourceFactory(connectionString);
            var releaseSource = factory.Create();
            builder.AddDependency(releaseSource);

            var sqlConnectionFactory = new SqlConnectionFactory(connectionString);
            var releaseInfoSource = new SqlServerReleaseInfoSource(sqlConnectionFactory);
            builder.AddDependency(releaseInfoSource);

            return builder;
        }
    }
}
