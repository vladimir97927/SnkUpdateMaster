using SnkUpateMaster.IntegrationTests.SeedWork;
using SnkUpdateMaster.Core;
using SnkUpdateMaster.Core.Installer;
using SnkUpdateMaster.Core.Integrity;
using SnkUpdateMaster.Core.VersionManager;
using SnkUpdateMaster.SqlServer;
using SnkUpdateMaster.SqlServer.Database;

namespace SnkUpateMaster.IntegrationTests
{
    [TestFixture]
    class UpdateManagerTests : TestBase
    {
        [Test]
        public async Task CheckAndInstallUpdateTest()
        {
            var sqlConnectionFactory = new SqlConnectionFactory(ConnectionString!);

            var updateSource = new SqlServerUpdateSource(sqlConnectionFactory);
            var downloader = new SqlServerUpdateDownloader(sqlConnectionFactory, DownloadsPath);

            var installer = new ZipInstaller(AppDir!);
            var currentVersionManager = new FileVersionManager();
            var integrityVerifier = new ShaIntegrityVerifier();

            var updateManager = new UpdateManager(
                currentVersionManager,
                updateSource,
                integrityVerifier,
                installer,
                downloader);

            var mockProgress = new Progress<double>();
            var isSuccess = await updateManager.CheckAndInstallUpdatesAsync(mockProgress);

            Assert.That(isSuccess, Is.True);

            var newVersion = await currentVersionManager.GetCurrentVersionAsync();

            Assert.That(newVersion, Is.Not.Null);
            Assert.That(newVersion, Is.EqualTo(new Version("1.0.2")));
        }
    }
}
