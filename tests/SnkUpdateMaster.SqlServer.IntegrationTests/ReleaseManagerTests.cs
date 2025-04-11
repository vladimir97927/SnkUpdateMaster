using SnkUpdateMaster.SqlServer.IntegrationTests.SeedWork;
using SnkUpdateMaster.Core.Integrity;
using SnkUpdateMaster.Core.ReleasePublisher;
using SnkUpdateMaster.Core.ReleasePublisher.Packager;
using SnkUpdateMaster.SqlServer.Database;
using NUnit.Framework;
using SnkUpdateMaster.SqlServer.Configuration;

namespace SnkUpdateMaster.SqlServer.IntegrationTests
{
    [TestFixture]
    internal class ReleaseManagerTests : TestBase
    {
        [Test]
        public async Task PublishReleaseTest()
        {
            var manager = new ReleaseManagerBuilder()
                .WithZipPackager(IntegrityProviderType.Sha256)
                .WithSqlServerReleaseSource(ConnectionString!)
                .Build();
            var newVersion = new Version(1, 1, 3);
            await manager.PulishReleaseAsync(AppDir, newVersion, new Progress<double>());

            var sqlConnectionFactory = new SqlConnectionFactory(ConnectionString!);
            var updateSource = new SqlServerUpdateSource(sqlConnectionFactory);
            var lastUpdates = await updateSource.GetLastUpdatesAsync();

            Assert.That(lastUpdates, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(lastUpdates.Version, Is.EqualTo(newVersion));
                Assert.That(lastUpdates.FileName, Is.EqualTo($"release-v{newVersion}.zip"));
            });
        }
    }
}
