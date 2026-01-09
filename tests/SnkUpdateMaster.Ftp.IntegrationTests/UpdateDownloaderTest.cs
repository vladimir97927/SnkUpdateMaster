using SnkUpdateMaster.Core.Parser;
using SnkUpdateMaster.Ftp.IntegrationTests.SeedWork;

namespace SnkUpdateMaster.Ftp.IntegrationTests
{
    [TestFixture]
    internal class UpdateDownloaderTest : TestBase
    {
        [Test]
        public async Task DownloadUpdatesFromFtpTest()
        {
            var ftpClientFactory = new AsyncFtpClientFactory(FtpHost, FtpUser, FtpPassword, FtpPort);
            var updateInfoFileParser = new JsonUpdateInfoParser();

            var ftpUpdateInfoProvider = new FtpUpdateInfoProvider(ftpClientFactory, updateInfoFileParser, UpdateInfoFilePath);
            var ftpUpdateDownloader = new FtpUpdateDownloader(ftpClientFactory, DownloadsDir);

            var updateInfo = await ftpUpdateInfoProvider.GetLastUpdatesAsync();

            Assert.That(updateInfo, Is.Not.Null);

            var path = await ftpUpdateDownloader.DownloadUpdateAsync(updateInfo, new Progress<double>());
            Assert.Multiple(() =>
            {
                Assert.That(path, Is.EqualTo($"{DownloadsDir}\\{updateInfo.FileName}"));
                Assert.That(File.Exists(path), Is.True);
            });
        }
    }
}
