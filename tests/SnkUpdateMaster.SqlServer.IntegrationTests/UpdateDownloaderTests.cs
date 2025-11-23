using SnkUpdateMaster.SqlServer.IntegrationTests.SeedWork;
using SnkUpdateMaster.Core.Integrity;
using SnkUpdateMaster.SqlServer.Database;
using NUnit.Framework;

namespace SnkUpdateMaster.SqlServer.IntegrationTests
{
    [TestFixture]
    class UpdateDownloaderTests : TestBase
    {
        [Test]
        public async Task DownloadUpdatesFromSqlDatabseTest()
        {
            var sqlConnectionFactory = new SqlConnectionFactory(ConnectionString!);

            var updateSource = new SqlServerUpdateInfoProvider(sqlConnectionFactory);
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
