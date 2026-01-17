using SnkUpdateMaster.Core;
using SnkUpdateMaster.Http.IntegrationTests.SeedWork;
using System.Net;

namespace SnkUpdateMaster.Http.IntegrationTests
{
    [TestFixture]
    internal class HttpUpdateDownloaderTests
    {
        [Test]
        public async Task DownloadUpdateAsync_DownloadsFileAndReportsProgress()
        {
            var tempRoot = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            var downloadsDir = Path.Combine(tempRoot, "downloads");
            var payload = new byte[64 * 1024];
            new Random(42).NextBytes(payload);

            try
            {
                var handler = new TestHttpMessageHandler((_, _) =>
                {
                    var response = new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new ByteArrayContent(payload)
                    };
                    response.Content.Headers.ContentLength = payload.Length;
                    return Task.FromResult(response);
                });
                using var httpClient = new HttpClient(handler)
                {
                    BaseAddress = new Uri("https://updates.example/")
                };
                var updateInfo = new UpdateInfo(
                    5,
                    "2.0.0",
                    "app.zip",
                    "checksum",
                    DateTime.UtcNow,
                    "releases");

                var progressValues = new List<double>();
                var progress = new Progress<double>(value => progressValues.Add(value));
                var downloader = new HttpUpdateDownloader(httpClient, downloadsDir);

                var localPath = await downloader.DownloadUpdateAsync(updateInfo, progress);

                Assert.That(File.Exists(localPath), Is.True);
                var downloadedBytes = await File.ReadAllBytesAsync(localPath);
                Assert.That(downloadedBytes, Is.EqualTo(payload));
                Assert.That(progressValues, Is.Not.Empty);
                Assert.That(progressValues[^1], Is.EqualTo(1.0));
                Assert.That(handler.Requests.Single().RequestUri, Is.EqualTo(new Uri("https://updates.example/releases/app.zip")));
            }
            finally
            {
                if (Directory.Exists(tempRoot))
                {
                    Directory.Delete(tempRoot, true);
                }
            }
        }

        [Test]
        public void DownloadUpdateAsync_ThrowsAndDoesNotLeaveFileOnHttpError()
        {
            var tempRoot = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            var downloadsDir = Path.Combine(tempRoot, "downloads");

            try
            {
                var handler = new TestHttpMessageHandler((_, _) =>
                {
                    var response = new HttpResponseMessage(HttpStatusCode.NotFound);
                    return Task.FromResult(response);
                });
                using var httpClient = new HttpClient(handler)
                {
                    BaseAddress = new Uri("https://updates.example/")
                };
                var updateInfo = new UpdateInfo(
                    7,
                    "3.0.0",
                    "missing.zip",
                    "checksum",
                    DateTime.UtcNow,
                    "releases");
                var downloader = new HttpUpdateDownloader(httpClient, downloadsDir);
                var expectedPath = Path.Combine(downloadsDir, updateInfo.FileName);

                Assert.That(
                    async () => await downloader.DownloadUpdateAsync(updateInfo),
                    Throws.TypeOf<HttpRequestException>());
                Assert.That(File.Exists(expectedPath), Is.False);
            }
            finally
            {
                if (Directory.Exists(tempRoot))
                {
                    Directory.Delete(tempRoot, true);
                }
            }
        }
    }
}
