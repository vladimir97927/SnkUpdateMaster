using SnkUpateMaster.IntegrationTests.SeedWork;
using SnkUpdateMaster.SqlServer;
using SnkUpdateMaster.SqlServer.Database;

namespace SnkUpateMaster.IntegrationTests
{
    [TestFixture]
    internal class UpdateSourceTests : TestBase
    {
        [Test]
        public async Task GetLastUpdatesFromSqlServerSourceTest()
        {
            var sqlConnectionFactory = new SqlConnectionFactory(ConnectionString!);
            var updateSource = new SqlServerUpdateSource(sqlConnectionFactory);
            var lastUpdates = await updateSource.GetLastUpdatesAsync();

            Assert.That(lastUpdates, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(lastUpdates.Id, Is.EqualTo(1));
                Assert.That(lastUpdates.Version.ToString(), Is.EqualTo("1.0.2"));
                Assert.That(lastUpdates.FileName, Is.EqualTo("AppTest.zip"));
                Assert.That(lastUpdates.Checksum, Is.EqualTo("da6499760769f971d007ae769ea91e3bd262b0a9fe6fa5a30ac63e853c7603cc"));
                Assert.That(lastUpdates.ReleaseDate, Is.EqualTo(new DateTime(2025, 3, 12)));
            });
        }
    }
}
