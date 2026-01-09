using SnkUpdateMaster.Core.Parser;
using System.Text.Json;

namespace SnkUpdateMaster.FileSystem.IntegrationTests
{
    [TestFixture]
    internal class FileSystemUpdateInfoProviderTests
    {
        [Test]
        public async Task GetLastUpdatesAsync_ReadsUpdateInfo()
        {
            var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDir);

            try
            {
                var manifestPath = Path.Combine(tempDir, "manifest.json");
                var manifest = new
                {
                    Id = 10,
                    Version = new Version("1.2.3"),
                    FileName = "app.zip",
                    Checksum = "checksum",
                    ReleaseDate = new DateTime(2025, 1, 1),
                    FileDir = "releases"
                };

                await File.WriteAllTextAsync(manifestPath, JsonSerializer.Serialize(manifest));

                var provider = new FileSystemUpdateInfoProvider(new JsonUpdateInfoParser(), manifestPath);

                var updateInfo = await provider.GetLastUpdatesAsync();

                Assert.That(updateInfo, Is.Not.Null);
                Assert.That(updateInfo.Id, Is.EqualTo(manifest.Id));
                Assert.That(updateInfo.Version, Is.EqualTo(manifest.Version));
                Assert.That(updateInfo.FileName, Is.EqualTo(manifest.FileName));
                Assert.That(updateInfo.FileDir, Is.EqualTo(manifest.FileDir));
            }
            finally
            {
                Directory.Delete(tempDir, true);
            }
        }

        [Test]
        public async Task GetLastUpdatesAsync_ReturnsNullWhenUpdateInfoMissing()
        {
            var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDir);

            try
            {
                var manifestPath = Path.Combine(tempDir, "manifest.json");
                var provider = new FileSystemUpdateInfoProvider(new JsonUpdateInfoParser(), manifestPath);

                var updateInfo = await provider.GetLastUpdatesAsync();

                Assert.That(updateInfo, Is.Null);
            }
            finally
            {
                Directory.Delete(tempDir, true);
            }
        }
    }
}
