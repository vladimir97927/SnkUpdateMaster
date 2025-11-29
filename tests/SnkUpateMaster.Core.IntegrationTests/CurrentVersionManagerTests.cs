using SnkUpateMaster.Core.IntegrationTests.SeedWork;
using SnkUpdateMaster.Core.VersionManager;

namespace SnkUpateMaster.Core.IntegrationTests
{
    [TestFixture]
    class CurrentVersionManagerTests : TestBase
    {
        [Test]
        public async Task FileVersionManagerTests()
        {
            var manager = new FileVersionManager();
            var currentVersion = await manager.GetCurrentVersionAsync();

            Assert.That(currentVersion, Is.Not.Null);
            Assert.That(currentVersion, Is.EqualTo(CurrentVersion));
        }

        [Test]
        public async Task UpdateCurrentVersionTest()
        {
            var manager = new FileVersionManager();
            var newVersion = new Version("1.0.2");

            await manager.UpdateCurrentVersionAsync(newVersion);
            var currentVersion = await manager.GetCurrentVersionAsync();

            Assert.That(currentVersion, Is.Not.Null);
            Assert.That(currentVersion, Is.EqualTo(newVersion));
        }
    }
}
