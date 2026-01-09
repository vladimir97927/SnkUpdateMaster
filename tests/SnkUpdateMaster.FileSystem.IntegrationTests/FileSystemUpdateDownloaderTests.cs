using SnkUpdateMaster.Core;

namespace SnkUpdateMaster.FileSystem.IntegrationTests
{
    [TestFixture]
    internal class FileSystemUpdateDownloaderTests
    {
        [Test]
        public async Task DownloadUpdateAsync_CopiesReleaseFileAndReportsProgress()
        {
            var tempRoot = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            var releasesDir = Path.Combine(tempRoot, "releases");
            var nestedDir = Path.Combine(releasesDir, "nested");
            var downloadsDir = Path.Combine(tempRoot, "downloads");

            Directory.CreateDirectory(nestedDir);

            try
            {
                var sourcePath = Path.Combine(nestedDir, "app.zip");
                var payload = new byte[128 * 1024];
                new Random(42).NextBytes(payload);
                await File.WriteAllBytesAsync(sourcePath, payload);

                var updateInfo = new UpdateInfo(
                    5,
                    "2.0.0",
                    "app.zip",
                    "checksum",
                    DateTime.UtcNow,
                    nestedDir);

                var progressValues = new List<double>();
                var progress = new Progress<double>(value => progressValues.Add(value));
                var downloader = new FileSystemUpdateDownloader(downloadsDir);

                var destinationPath = await downloader.DownloadUpdateAsync(updateInfo, progress);

                Assert.That(File.Exists(destinationPath), Is.True);
                var destinationBytes = await File.ReadAllBytesAsync(destinationPath);
                Assert.That(destinationBytes, Is.EqualTo(payload));
                Assert.That(progressValues, Is.Not.Empty);
                Assert.That(progressValues[^1], Is.EqualTo(1.0));
            }
            finally
            {
                Directory.Delete(tempRoot, true);
            }
        }

        [Test]
        public void DownloadUpdateAsync_ThrowsWhenReleaseMissing()
        {
            var tempRoot = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            var releasesDir = Path.Combine(tempRoot, "releases");
            var downloadsDir = Path.Combine(tempRoot, "downloads");
            Directory.CreateDirectory(releasesDir);

            try
            {
                var updateInfo = new UpdateInfo(
                    5,
                    "2.0.0",
                    "missing.zip",
                    "checksum",
                    DateTime.UtcNow,
                    null);

                var downloader = new FileSystemUpdateDownloader(downloadsDir);

                Assert.That(
                    async () => await downloader.DownloadUpdateAsync(updateInfo),
                    Throws.InstanceOf<FileNotFoundException>());
            }
            finally
            {
                Directory.Delete(tempRoot, true);
            }
        }
    }
}
