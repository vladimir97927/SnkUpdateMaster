using SnkUpdateMaster.Core;
using SnkUpdateMaster.Core.Files;
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
            var updateInfoFileParser = new JsonUpdateInfoFileParser();

            var updateManager = new UpdateManagerBuilder()
                .WithFileCurrentVersionManager()
                .WithSha256IntegrityVerifier()
                .WithZipInstaller(AppDir)
                .WithFtpUpdateInfoProvider(updateInfoFileParser, ftpClientFactory, UpdateInfoFilePath)
                .WithFtpUpdateDownloader(ftpClientFactory, DownloadsDir)
                .Build();

            var mockProgress = new Progress<double>();
            var isSuccess = await updateManager.CheckAndInstallUpdatesAsync(mockProgress);

            Assert.That(isSuccess, Is.True);

            var newVersion = await updateManager.GetCurrentVersionAsync();

            Assert.That(newVersion, Is.Not.Null);
            Assert.That(newVersion.ToString(), Is.EqualTo("1.0.1"));
        }
    }
}
