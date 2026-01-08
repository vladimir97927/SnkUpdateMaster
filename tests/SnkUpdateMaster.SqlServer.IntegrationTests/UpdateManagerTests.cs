using NUnit.Framework;
using SnkUpdateMaster.Core;
using SnkUpdateMaster.SqlServer.Configuration;
using SnkUpdateMaster.SqlServer.Database;
using SnkUpdateMaster.SqlServer.IntegrationTests.SeedWork;

namespace SnkUpdateMaster.SqlServer.IntegrationTests
{
    [TestFixture]
    class UpdateManagerTests : TestBase
    {
        [Test]
        public async Task CheckAndInstallUpdateTest()
        {
            var sqlConnectionFactory = new SqlConnectionFactory(ConnectionString);
            var loggerFactory = new TestLoggerFactory();

            var updateManager = new UpdateManagerBuilder()
                .WithFileCurrentVersionManager()
                .WithSha256IntegrityVerifier()
                .WithZipInstaller(AppDir)
                .WithSqlServerUpdateInfoProvider(sqlConnectionFactory)
                .WithSqlServerUpdateDownloader(sqlConnectionFactory, DownloadsPath)
                .WithLogger(loggerFactory)
                .Build();

            var mockProgress = new Progress<double>();
            var isSuccess = await updateManager.CheckAndInstallUpdatesAsync(mockProgress);

            Assert.That(isSuccess, Is.True);

            var newVersion = await updateManager.GetCurrentVersionAsync();

            var categoryName = typeof(UpdateManager).FullName ?? string.Empty;
            var updateManagerLogger = (TestLogger<object>)loggerFactory.Loggers[categoryName];

            Assert.That(newVersion, Is.Not.Null);
            Assert.That(newVersion, Is.EqualTo(new Version("1.0.2")));
            Assert.That(updateManagerLogger, Is.Not.Null);
            Assert.That(updateManagerLogger.Entries, Is.Not.Empty);
            Assert.That(updateManagerLogger.Entries.Any(e => e.Message.Contains("Update process finished successfully")), Is.True);
        }
    }
}
