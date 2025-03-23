using SnkUpateMaster.IntegrationTests.SeedWork;
using SnkUpdateMaster.Core.Integrity;
using SnkUpdateMaster.Core.ReleasePublisher;
using SnkUpdateMaster.Core.ReleasePublisher.Packager;
using SnkUpdateMaster.SqlServer;

namespace SnkUpateMaster.IntegrationTests
{
    [TestFixture]
    internal class ReleaseManagerTests : TestBase
    {
        [Test]
        public async Task PublishReleaseTest()
        {
            var integrityProvider = new ShaIntegrityProvider();
            var releasePackager = new ZipReleasePackager(integrityProvider);

            var releaseSourceFactory = new SqlServerReleaseSourceFactory(ConnectionString!);
            var releaseSource = releaseSourceFactory.Create();

            var manager = new ReleaseManager(releasePackager, releaseSource);
            var newVersion = new Version(1, 1, 3);
            await manager.PulishReleaseAsync(AppDir, newVersion, new Progress<double>());

            var factory = new SqlServerUpdateSourceFactory(ConnectionString!);
            var updateSource = factory.Create();
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
