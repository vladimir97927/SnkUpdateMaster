using SnkUpdateMaster.Core;
using SnkUpdateMaster.Core.Downloader;
using SnkUpdateMaster.Core.UpdateSource;
using SnkUpdateMaster.SqlServer.Database;

namespace SnkUpdateMaster.SqlServer.Configuration
{
    public static class UpdateManagerBuilderSqlServerExtension
    {
        public static UpdateManagerBuilder WithSqlServerUpdateProvider(
            this UpdateManagerBuilder builder,
            string connectionString,
            string downloadsDir)
        {
            var sqlConnectionFactory = new SqlConnectionFactory(connectionString);
            var updateSource = new SqlServerUpdateSource(sqlConnectionFactory);
            var downloader = new SqlServerUpdateDownloader(sqlConnectionFactory, downloadsDir);

            builder.AddDependency<IUpdateSource>(updateSource);
            builder.AddDependency<IUpdateDownloader>(downloader);

            return builder;
        }
    }
}
