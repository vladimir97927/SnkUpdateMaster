using SnkUpdateMaster.Core.Files;
using SnkUpdateMaster.Ftp.IntegrationTests.SeedWork;

namespace SnkUpdateMaster.Ftp.IntegrationTests
{
    [TestFixture]
    internal class UpdateInfoProviderTests : TestBase
    {
        [Test]
        public async Task GetLastUpdatesFromFtpSourceTest()
        {
            var ftpClientFactory = new AsyncFtpClientFactory(FtpHost, FtpUser, FtpPassword, FtpPort);
            var updateInfoFileParser = new JsonUpdateInfoFileParser();

            var ftpUpdateInfoProvider = new FtpUpdateInfoProvider(ftpClientFactory, updateInfoFileParser, UpdateInfoFilePath);

            var updateInfo = await ftpUpdateInfoProvider.GetLastUpdatesAsync();

            Assert.That(updateInfo, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(updateInfo.Id, Is.EqualTo(1));
                Assert.That(updateInfo.Version.ToString(), Is.EqualTo("1.0.1"));
                Assert.That(updateInfo.FileName, Is.EqualTo("release-1.0.1.zip"));
                Assert.That(updateInfo.Checksum, Is.EqualTo("9d71e8d1b76e8a30bb11d671e76bd7512a20e93a0a09f78fa1a5a20b8ef79cda"));
                Assert.That(updateInfo.ReleaseDate, Is.EqualTo(new DateTime(2025, 11, 29)));
                Assert.That(updateInfo.FileDir, Is.EqualTo("/"));
            });
        }
    }
}
