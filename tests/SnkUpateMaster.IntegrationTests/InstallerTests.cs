using SnkUpateMaster.Core.IntegrationTests.SeedWork;
using SnkUpdateMaster.Core.Installer;

namespace SnkUpateMaster.Core.IntegrationTests
{
    [TestFixture]
    internal class InstallerTests : TestBase
    {
        [Test]
        public async Task ZipInstallerTest()
        {
            var zipInstaller = new ZipInstaller(AppDir);
            var mockProgress = new Progress<double>();
            var updateFilePath = @"SeedWork\TestApp.zip";

            await zipInstaller.InstallAsync(updateFilePath, mockProgress);

            var directories = Directory.GetDirectories(AppDir);
            Assert.Multiple(() =>
            {
                Assert.That(directories, Does.Contain("testApp\\testFolder"));
                Assert.That(directories, Does.Contain("testApp\\backup"));
            });
        }
    }
}
