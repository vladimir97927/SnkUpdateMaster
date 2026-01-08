using Microsoft.Data.SqlClient;
using NUnit.Framework;
using SnkUpdateMaster.Core;
using SnkUpdateMaster.SqlServer.Database;
using SnkUpdateMaster.SqlServer.IntegrationTests.SeedWork;

namespace SnkUpdateMaster.SqlServer.IntegrationTests
{
    [TestFixture]
    internal class SqlServerUpdateDownloaderLoggerTests : TestBase
    {
        [Test]
        public async Task DownloadUpdateAsync_LogsStartAndSuccess()
        {
            var sqlConnectionFactory = new SqlConnectionFactory(ConnectionString);
            var infoLogger = new TestLogger<SqlServerUpdateInfoProvider>();
            var infoProvider = new SqlServerUpdateInfoProvider(sqlConnectionFactory, infoLogger);
            var updateInfo = await infoProvider.GetLastUpdatesAsync();

            var logger = new TestLogger<SqlServerUpdateDownloader>();
            var downloader = new SqlServerUpdateDownloader(sqlConnectionFactory, DownloadsPath, logger);

            var filePath = await downloader.DownloadUpdateAsync(updateInfo!);

            Assert.That(File.Exists(filePath), Is.True);
            Assert.That(logger.Entries.Any(e => e.Message.Contains("Starting SQL Server download for update")), Is.True);
            Assert.That(logger.Entries.Any(e => e.Message.Contains("Successfully downloaded update from SQL Server")), Is.True);
        }

        [Test]
        public void DownloadUpdateAsync_LogsErrorWhenFileMissing()
        {
            var sqlConnectionFactory = new SqlConnectionFactory(ConnectionString);
            var updateInfo = new UpdateInfo(
                1,
                new Version("1.0.2"),
                "AppTest.zip",
                "unused",
                DateTime.UtcNow);

            using var connection = new SqlConnection(ConnectionString);
            connection.Open();
            using (var command = new SqlCommand("DELETE FROM [dbo].[UpdateFile] WHERE [UpdateInfoId] = 1", connection))
            {
                command.ExecuteNonQuery();
            }

            var logger = new TestLogger<SqlServerUpdateDownloader>();
            var downloader = new SqlServerUpdateDownloader(sqlConnectionFactory, DownloadsPath, logger);

            Assert.That(
                async () => await downloader.DownloadUpdateAsync(updateInfo),
                Throws.TypeOf<KeyNotFoundException>());
            Assert.That(logger.Entries.Any(e => e.Message.Contains("Update file with UpdateInfoId")), Is.True);
        }
    }
}
