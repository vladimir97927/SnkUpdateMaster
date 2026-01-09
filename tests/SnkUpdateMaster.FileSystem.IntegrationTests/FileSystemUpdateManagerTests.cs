using SnkUpdateMaster.Core;
using SnkUpdateMaster.Core.Integrity;
using SnkUpdateMaster.Core.Parser;
using SnkUpdateMaster.FileSystem.Configuration;
using System.IO.Compression;
using System.Text.Json;

namespace SnkUpdateMaster.FileSystem.IntegrationTests
{
    [TestFixture]
    internal class FileSystemUpdateManagerTests
    {
        [Test]
        public async Task CheckAndInstallUpdatesAsync_UsesFileSystemProviders()
        {
            var originalDirectory = Environment.CurrentDirectory;
            var tempRoot = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            var releasesDir = Path.Combine(tempRoot, "releases");
            var downloadsDir = Path.Combine(tempRoot, "downloads");
            var appDir = Path.Combine(tempRoot, "app");

            Directory.CreateDirectory(releasesDir);
            Directory.CreateDirectory(appDir);

            try
            {
                Environment.CurrentDirectory = tempRoot;
                await File.WriteAllTextAsync(Path.Combine(tempRoot, "version"), "1.0.0");

                var updateZipPath = Path.Combine(releasesDir, "update.zip");
                CreateUpdateArchive(updateZipPath);

                var checksum = new ShaChecksumCalculator().ComputeChecksum(updateZipPath);
                var manifestPath = Path.Combine(releasesDir, "manifest.json");

                var manifest = new
                {
                    Id = 1,
                    Version = "1.1.0",
                    FileName = "update.zip",
                    Checksum = checksum,
                    ReleaseDate = new DateTime(2025, 1, 1),
                    FileDir = releasesDir
                };

                await File.WriteAllTextAsync(manifestPath, JsonSerializer.Serialize(manifest));

                var updateManager = new UpdateManagerBuilder()
                    .WithFileCurrentVersionManager()
                    .WithSha256IntegrityVerifier()
                    .WithZipInstaller(appDir)
                    .WithFileSystemUpdateInfoProvider(new JsonUpdateInfoParser(), manifestPath)
                    .WithFileSystemUpdateDownloader(downloadsDir)
                    .Build();

                var updated = await updateManager.CheckAndInstallUpdatesAsync();

                Assert.That(updated, Is.True);
                Assert.That(File.Exists(Path.Combine(appDir, "updated.txt")), Is.True);
                Assert.That(await File.ReadAllTextAsync(Path.Combine(tempRoot, "version")), Is.EqualTo("1.1.0"));
            }
            finally
            {
                Environment.CurrentDirectory = originalDirectory;
                if (Directory.Exists(tempRoot))
                {
                    Directory.Delete(tempRoot, true);
                }
            }
        }

        private static void CreateUpdateArchive(string zipPath)
        {
            using var archive = ZipFile.Open(zipPath, ZipArchiveMode.Create);
            var entry = archive.CreateEntry("updated.txt");
            using var entryStream = entry.Open();
            var payload = new byte[128];
            new Random(123).NextBytes(payload);
            entryStream.Write(payload, 0, payload.Length);
        }
    }
}
