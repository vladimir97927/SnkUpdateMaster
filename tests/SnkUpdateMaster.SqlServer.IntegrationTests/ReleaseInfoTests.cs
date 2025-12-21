using SnkUpdateMaster.SqlServer.IntegrationTests.SeedWork;
using SnkUpdateMaster.SqlServer.Database;
using NUnit.Framework;

namespace SnkUpdateMaster.SqlServer.IntegrationTests
{
    [TestFixture]
    internal class ReleaseInfoTests : TestBase
    {
        [Test]
        public async Task GetReleaseInfosTest()
        {
            var sqlConnectionFactory = new SqlConnectionFactory(ConnectionString);
            var releaseInfoSource = new SqlServerReleaseInfoSource(sqlConnectionFactory);

            var releases = await releaseInfoSource.GetReleaseInfosPagedAsync();

            Assert.That(releases.Data.Count, Is.EqualTo(2));
            Assert.Multiple(() =>
            {
                Assert.That(releases.TotalCount, Is.EqualTo(2));
                Assert.That(releases.PageNumber, Is.Null);
                Assert.That(releases.PageSize, Is.Null);
            });
        }

        [Test]
        public async Task GetPagedReleaseInfosTest()
        {
            var sqlConnectionFactory = new SqlConnectionFactory(ConnectionString);
            var releaseInfoSource = new SqlServerReleaseInfoSource(sqlConnectionFactory);

            var releases = await releaseInfoSource.GetReleaseInfosPagedAsync(1, 1);

            Assert.That(releases.Data.Count, Is.EqualTo(1));
            Assert.Multiple(() =>
            {
                Assert.That(releases.TotalCount, Is.EqualTo(2));
                Assert.That(releases.PageNumber, Is.EqualTo(1));
                Assert.That(releases.PageSize, Is.EqualTo(1));
            });

            releases = await releaseInfoSource.GetReleaseInfosPagedAsync(2, 1);

            Assert.That(releases.Data.Count, Is.EqualTo(1));
            Assert.Multiple(() =>
            {
                Assert.That(releases.TotalCount, Is.EqualTo(2));
                Assert.That(releases.PageNumber, Is.EqualTo(2));
                Assert.That(releases.PageSize, Is.EqualTo(1));
            });
        }
    }
}
