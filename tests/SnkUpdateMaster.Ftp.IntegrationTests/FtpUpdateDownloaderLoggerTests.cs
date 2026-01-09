using SnkUpdateMaster.Core;
using SnkUpdateMaster.Core.Parser;
using SnkUpdateMaster.Ftp.IntegrationTests.SeedWork;

namespace SnkUpdateMaster.Ftp.IntegrationTests
{
    [TestFixture]
    internal class FtpUpdateDownloaderLoggerTests : TestBase
    {
        [Test]
        public async Task DownloadUpdateAsync_LogsStartAndSuccess()
        {
            var ftpClientFactory = new AsyncFtpClientFactory(FtpHost, FtpUser, FtpPassword, FtpPort);
            var updateInfoFileParser = new JsonUpdateInfoParser();
            var infoLogger = new TestLogger<FtpUpdateInfoProvider>();
            var infoProvider = new FtpUpdateInfoProvider(ftpClientFactory, updateInfoFileParser, UpdateInfoFilePath, infoLogger);
            var updateInfo = await infoProvider.GetLastUpdatesAsync();

            var logger = new TestLogger<FtpUpdateDownloader>();
            var downloader = new FtpUpdateDownloader(ftpClientFactory, DownloadsDir, logger);

            var filePath = await downloader.DownloadUpdateAsync(updateInfo!);

            Assert.That(File.Exists(filePath), Is.True);
            Assert.That(logger.Entries.Any(e => e.Message.Contains("Starting download of update")), Is.True);
            Assert.That(logger.Entries.Any(e => e.Message.Contains("Successfully downloaded update")), Is.True);
        }

        [Test]
        public void DownloadUpdateAsync_LogsErrorWhenFileMissing()
        {
            var ftpClientFactory = new AsyncFtpClientFactory(FtpHost, FtpUser, FtpPassword, FtpPort);
            var logger = new TestLogger<FtpUpdateDownloader>();
            var downloader = new FtpUpdateDownloader(ftpClientFactory, DownloadsDir, logger);
            var updateInfo = new UpdateInfo(
                99,
                new Version("9.9.9"),
                "missing.zip",
                "unused",
                DateTime.UtcNow,
                "/");

            Assert.That(
                async () => await downloader.DownloadUpdateAsync(updateInfo),
                Throws.TypeOf<FileNotFoundException>());
            Assert.That(logger.Entries.Any(e => e.Message.Contains("Update file not found on FTP server")), Is.True);
        }
    }
}
