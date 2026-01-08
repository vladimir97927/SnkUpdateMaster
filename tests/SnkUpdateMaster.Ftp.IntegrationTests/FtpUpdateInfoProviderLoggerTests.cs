using SnkUpdateMaster.Core.Files;
using SnkUpdateMaster.Ftp.IntegrationTests.SeedWork;

namespace SnkUpdateMaster.Ftp.IntegrationTests
{
    [TestFixture]
    internal class FtpUpdateInfoProviderLoggerTests : TestBase
    {
        [Test]
        public async Task GetLastUpdatesAsync_LogsStartAndSuccess()
        {
            var ftpClientFactory = new AsyncFtpClientFactory(FtpHost, FtpUser, FtpPassword, FtpPort);
            var updateInfoFileParser = new JsonUpdateInfoFileParser();
            var logger = new TestLogger<FtpUpdateInfoProvider>();
            var provider = new FtpUpdateInfoProvider(ftpClientFactory, updateInfoFileParser, UpdateInfoFilePath, logger);

            var updateInfo = await provider.GetLastUpdatesAsync();

            Assert.That(updateInfo, Is.Not.Null);
            Assert.That(logger.Entries.Any(e => e.Message.Contains("Starting to get update info from FTP server.")), Is.True);
            Assert.That(logger.Entries.Any(e => e.Message.Contains("Successfully retrieved update info")), Is.True);
        }

        [Test]
        public async Task GetLastUpdatesAsync_LogsWarningWhenFileMissing()
        {
            var ftpClientFactory = new AsyncFtpClientFactory(FtpHost, FtpUser, FtpPassword, FtpPort);
            var updateInfoFileParser = new JsonUpdateInfoFileParser();
            var logger = new TestLogger<FtpUpdateInfoProvider>();
            var provider = new FtpUpdateInfoProvider(ftpClientFactory, updateInfoFileParser, "/missing.json", logger);

            var updateInfo = await provider.GetLastUpdatesAsync();

            Assert.That(updateInfo, Is.Null);
            Assert.That(logger.Entries.Any(e => e.Message.Contains("Update info file not found at path")), Is.True);
        }
    }
}
