using SnkUpateMaster.IntegrationTests.SeedWork;
using SnkUpdateMaster.Core.Installer;
using SnkUpdateMaster.Core.Integrity;
using SnkUpdateMaster.SqlServer;

namespace SnkUpateMaster.IntegrationTests
{
    [TestFixture]
    class InstallerTests : TestBase
    {
        [Test]
        public async Task ZipInstallerTest()
        {
            var updateSourceFactory = new SqlServerUpdateSourceFactory(ConnectionString!);
            var downloaderFactory = new SqlServerUpdateDownloaderFactory(ConnectionString!, DownloadsPath);

            var updateSource = updateSourceFactory.Create();
            var downloader = downloaderFactory.Create();

            var updateInfo = await updateSource.GetLastUpdatesAsync();

            Assert.That(updateInfo, Is.Not.Null);

            var path = await downloader.DownloadUpdateAsync(updateInfo, new Progress<double>());
            Assert.That(path, Is.EqualTo($"{DownloadsPath}\\{updateInfo.FileName}"));

            var integrityVerifier = new ShaIntegrityVerifier();
            var isSuccess = integrityVerifier.VerifyFile(path, updateInfo.Checksum);

            Assert.That(isSuccess, Is.True);

            var installer = new ZipInstaller(AppDir!);
            await installer.InstallAsync(path, new Progress<double>());

            Assert.That(File.Exists(Path.Combine(AppDir, "TestApp", "TestApp.exe")), Is.True);
        }
    }
}
