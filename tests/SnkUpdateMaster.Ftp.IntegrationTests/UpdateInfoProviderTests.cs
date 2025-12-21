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
                Assert.That(updateInfo.Checksum, Is.EqualTo("da6499760769f971d007ae769ea91e3bd262b0a9fe6fa5a30ac63e853c7603cc"));
                Assert.That(updateInfo.ReleaseDate, Is.EqualTo(new DateTime(2025, 11, 29)));
                Assert.That(updateInfo.FileDir, Is.EqualTo("/"));
            });
        }
    }
}
