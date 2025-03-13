using SnkUpateMaster.IntegrationTests.SeedWork;
using SnkUpdateMaster.Core.Integrity;
using SnkUpdateMaster.SqlServer;

namespace SnkUpateMaster.IntegrationTests
{
    [TestFixture]
    class UpdateDownloaderTests : TestBase
    {
        [Test]
        public async Task DownloadUpdatesFromSqlDatabseTest()
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
        }
    }
}
