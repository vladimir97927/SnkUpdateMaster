using SnkUpateMaster.IntegrationTests.SeedWork;
using SnkUpdateMaster.Core.Integrity;
using SnkUpdateMaster.SqlServer;
using SnkUpdateMaster.SqlServer.Configuration.Data;

namespace SnkUpateMaster.IntegrationTests
{
    [TestFixture]
    class UpdateDownloaderTests : TestBase
    {
        [Test]
        public async Task DownloadUpdatesFromSqlDatabseTest()
        {
            var sqlConnectionFactory = new SqlConnectionFactory(ConnectionString!);

            var updateSource = new SqlServerUpdateSource(sqlConnectionFactory);
            var downloader = new SqlServerUpdateDownloader(sqlConnectionFactory, DownloadsPath);

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
