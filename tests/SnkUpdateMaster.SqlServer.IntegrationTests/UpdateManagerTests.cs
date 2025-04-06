using SnkUpdateMaster.SqlServer.IntegrationTests.SeedWork;
using SnkUpdateMaster.Core;
using NUnit.Framework;
using SnkUpdateMaster.SqlServer.Configuration;

namespace SnkUpdateMaster.SqlServer.IntegrationTests
{
    [TestFixture]
    class UpdateManagerTests : TestBase
    {
        [Test]
        public async Task CheckAndInstallUpdateTest()
        {
            var updateManager = new UpdateManagerBuilder()
                .WithFileCurrentVersionManager()
                .WithSha256IntegrityVerifier()
                .WithZipInstaller(AppDir)
                .WithSqlServerUpdateProvider(ConnectionString!, DownloadsPath)
                .Build();

            var mockProgress = new Progress<double>();
            var isSuccess = await updateManager.CheckAndInstallUpdatesAsync(mockProgress);

            Assert.That(isSuccess, Is.True);

            var newVersion = await updateManager.GetCurrentVersionAsync();

            Assert.That(newVersion, Is.Not.Null);
            Assert.That(newVersion, Is.EqualTo(new Version("1.0.2")));
        }
    }
}
