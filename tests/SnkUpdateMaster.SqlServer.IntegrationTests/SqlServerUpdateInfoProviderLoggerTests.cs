using Microsoft.Data.SqlClient;
using NUnit.Framework;
using SnkUpdateMaster.SqlServer.Database;
using SnkUpdateMaster.SqlServer.IntegrationTests.SeedWork;

namespace SnkUpdateMaster.SqlServer.IntegrationTests
{
    [TestFixture]
    internal class SqlServerUpdateInfoProviderLoggerTests : TestBase
    {
        [Test]
        public async Task GetLastUpdatesAsync_LogsStartAndSuccess()
        {
            var sqlConnectionFactory = new SqlConnectionFactory(ConnectionString);
            var logger = new TestLogger<SqlServerUpdateInfoProvider>();
            var provider = new SqlServerUpdateInfoProvider(sqlConnectionFactory, logger);

            var updateInfo = await provider.GetLastUpdatesAsync();

            Assert.That(updateInfo, Is.Not.Null);
            Assert.That(logger.Entries.Any(e => e.Message.Contains("Requesting latest update info from SQL Server.")), Is.True);
            Assert.That(logger.Entries.Any(e => e.Message.Contains("Latest update loaded from SQL Server")), Is.True);
        }

        [Test]
        public async Task GetLastUpdatesAsync_LogsWhenNoUpdates()
        {
            await using var connection = new SqlConnection(ConnectionString);
            await connection.OpenAsync();
            await using (var command = new SqlCommand("DELETE FROM [dbo].[UpdateInfo]", connection))
            {
                await command.ExecuteNonQueryAsync();
            }

            var sqlConnectionFactory = new SqlConnectionFactory(ConnectionString);
            var logger = new TestLogger<SqlServerUpdateInfoProvider>();
            var provider = new SqlServerUpdateInfoProvider(sqlConnectionFactory, logger);

            var updateInfo = await provider.GetLastUpdatesAsync();

            Assert.That(updateInfo, Is.Null);
            Assert.That(logger.Entries.Any(e => e.Message.Contains("No updates found in SQL Server UpdateInfo table.")), Is.True);
        }
    }
}
