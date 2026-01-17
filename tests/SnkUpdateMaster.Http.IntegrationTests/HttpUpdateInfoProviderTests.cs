using SnkUpdateMaster.Core.Parser;
using SnkUpdateMaster.Http.IntegrationTests.SeedWork;
using System.Net;
using System.Text;

namespace SnkUpdateMaster.Http.IntegrationTests
{
    [TestFixture]
    internal class HttpUpdateInfoProviderTests
    {
        private const string UpdateInfoJson = """
            {
              "id": 1,
              "version": "1.0.1",
              "fileName": "release-1.0.1.zip",
              "fileDir": "/",
              "checksum": "checksum",
              "releaseDate": "2025-11-29"
            }
            """;

        [Test]
        public async Task GetLastUpdatesAsync_StartAndSuccess()
        {
            var handler = new TestHttpMessageHandler((_, _) =>
            {
                var response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new ByteArrayContent(Encoding.UTF8.GetBytes(UpdateInfoJson))
                };
                return Task.FromResult(response);
            });

            using var httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri("https://updates.example/")
            };

            var parser = new JsonUpdateInfoParser();
            var provider = new HttpUpdateInfoProvider(httpClient, parser, "manifest.json");

            var updateInfo = await provider.GetLastUpdatesAsync();

            Assert.That(updateInfo, Is.Not.Null);
            Assert.That(updateInfo.FileName, Is.EqualTo("release-1.0.1.zip"));
        }

        [Test]
        public async Task GetLastUpdatesAsync_ReturnsNullWhenNotFound()
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
            var parser = new JsonUpdateInfoParser();
            var provider = new HttpUpdateInfoProvider(httpClient, parser, "manifest.json");

            var updateInfo = await provider.GetLastUpdatesAsync();

            Assert.That(updateInfo, Is.Null);
        }

        [Test]
        public async Task GetLastUpdatesAsync_ReturnsNullWhenContentEmpty()
        {
            var handler = new TestHttpMessageHandler((_, _) =>
            {
                var response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new ByteArrayContent([])
                };
                return Task.FromResult(response);
            });
            using var httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri("https://updates.example/")
            };
            var parser = new JsonUpdateInfoParser();
            var provider = new HttpUpdateInfoProvider(httpClient, parser, "manifest.json");

            var updateInfo = await provider.GetLastUpdatesAsync();

            Assert.That(updateInfo, Is.Null);
        }
    }
}
