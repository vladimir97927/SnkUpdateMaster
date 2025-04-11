using SnkUpdateMaster.Core.ReleasePublisher;

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

            return builder;
        }
    }
}
