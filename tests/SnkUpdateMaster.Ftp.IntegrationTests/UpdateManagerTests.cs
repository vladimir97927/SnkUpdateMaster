using SnkUpdateMaster.Core;
using SnkUpdateMaster.Core.Parser;
using SnkUpdateMaster.Ftp.Configuration;
using SnkUpdateMaster.Ftp.IntegrationTests.SeedWork;

namespace SnkUpdateMaster.Ftp.IntegrationTests
{
    [TestFixture]
    internal class UpdateManagerTests : TestBase
    {
        [Test]
        public async Task CheckAndInstallUpdateTest()
        {
            var ftpClientFactory = new AsyncFtpClientFactory(FtpHost, FtpUser, FtpPassword, FtpPort);
            var updateInfoFileParser = new JsonUpdateInfoParser();
            var loggerFactory = new TestLoggerFactory();

            var updateManager = new UpdateManagerBuilder()
                .WithFileCurrentVersionManager()
                .WithSha256IntegrityVerifier()
                .WithZipInstaller(AppDir)
                .WithFtpUpdateInfoProvider(updateInfoFileParser, ftpClientFactory, UpdateInfoFilePath)
                .WithFtpUpdateDownloader(ftpClientFactory, DownloadsDir)
                .WithLogger(loggerFactory)
                .Build();

            var mockProgress = new Progress<double>();
            var isSuccess = await updateManager.CheckAndInstallUpdatesAsync(mockProgress);

            Assert.That(isSuccess, Is.True);

            var newVersion = await updateManager.GetCurrentVersionAsync();

            var categoryName = typeof(UpdateManager).FullName ?? string.Empty;
            var updateManagerLogger = (TestLogger<object>)loggerFactory.Loggers[categoryName];

            Assert.That(newVersion, Is.Not.Null);
            Assert.That(newVersion.ToString(), Is.EqualTo("1.0.1"));
            Assert.That(updateManagerLogger, Is.Not.Null);
            Assert.That(updateManagerLogger.Entries, Is.Not.Empty);
            Assert.That(updateManagerLogger.Entries.Any(e => e.Message.Contains("Update process finished successfully")), Is.True);
        }
    }
}
