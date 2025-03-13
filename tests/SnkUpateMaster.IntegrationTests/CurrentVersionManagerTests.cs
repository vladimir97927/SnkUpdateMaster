using SnkUpateMaster.IntegrationTests.SeedWork;
using SnkUpdateMaster.Core.VersionManager;

namespace SnkUpateMaster.IntegrationTests
{
    [TestFixture]
    class CurrentVersionManagerTests : TestBase
    {
        [Test]
        public async Task FileVersionManagerTests()
        {
            var stream = File.Create("version");
            stream.Close();
            var manager = new FileVersionManager();
            var testVersion = new Version("1.0.0");
            await manager.UpdateCurrentVersionAsync(testVersion);
            var currentVersion = await manager.GetCurrentVersionAsync();

            Assert.That(currentVersion, Is.Not.Null);
            Assert.That(currentVersion, Is.EqualTo(testVersion));
        }
    }
}
