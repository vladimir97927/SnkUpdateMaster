using SnkUpateMaster.Core.IntegrationTests.SeedWork;
using SnkUpdateMaster.Core.Installer;

namespace SnkUpateMaster.Core.IntegrationTests
{
    [TestFixture]
    internal class InstallerTests : TestBase
    {
        private string _zipUpdatePath = string.Empty;

        [SetUp]
        public new void BeforeEachTest()
        {
            _zipUpdatePath = ZipUpdateHelper.CreateMockZipUpdate();
        }

        [TearDown]
        public new void AfterEachTest()
        {
            if (File.Exists(_zipUpdatePath))
            {
                File.Delete(_zipUpdatePath);
            }
        }

        [Test]
        public async Task ZipInstallerTest()
        {
            var zipInstaller = new ZipInstaller(AppDir);
            var mockProgress = new Progress<double>();

            await zipInstaller.InstallAsync(_zipUpdatePath, mockProgress);

            var directories = Directory.GetDirectories(AppDir);
            Assert.Multiple(() =>
            {
                Assert.That(directories, Does.Contain("testApp\\testFolder"));
                Assert.That(Directory.Exists(BackupDir), Is.False);
                Assert.That(File.Exists(Path.Combine(AppDir, "testlib_3.dll")), Is.True);
            });
        }

        [Test]
        public async Task ZipInstallerWhenDiskSpaceIsInsufficientTest()
        {
            var diskSpaceProvider = new FakeDiskSpaceProvider(10);
            var zipInstaller = new ZipInstaller(AppDir, diskSpaceProvider);

            Assert.ThrowsAsync<IOException>(() => zipInstaller.InstallAsync(_zipUpdatePath));
        }
    }
}
