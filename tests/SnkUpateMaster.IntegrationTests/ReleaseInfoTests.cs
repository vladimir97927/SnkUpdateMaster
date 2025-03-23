using SnkUpateMaster.IntegrationTests.SeedWork;
using SnkUpdateMaster.SqlServer;
using SnkUpdateMaster.SqlServer.Database;

namespace SnkUpateMaster.IntegrationTests
{
    [TestFixture]
    internal class ReleaseInfoTests : TestBase
    {
        [Test]
        public async Task GetReleaseInfosTest()
        {
            var sqlConnectionFactory = new SqlConnectionFactory(ConnectionString!);
            var releaseInfoSource = new SqlServerReleaseInfoSource(sqlConnectionFactory);

            var releases = await releaseInfoSource.GetReleaseInfosPagedAsync();

            Assert.That(releases.Count, Is.EqualTo(2));
        }

        [Test]
        public async Task GetPagedReleaseInfosTest()
        {
            var sqlConnectionFactory = new SqlConnectionFactory(ConnectionString!);
            var releaseInfoSource = new SqlServerReleaseInfoSource(sqlConnectionFactory);

            var releases = await releaseInfoSource.GetReleaseInfosPagedAsync(1, 1);

            Assert.That(releases.Count, Is.EqualTo(1));

            releases = await releaseInfoSource.GetReleaseInfosPagedAsync(2, 1);

            Assert.That(releases.Count, Is.EqualTo(1));
        }
    }
}
