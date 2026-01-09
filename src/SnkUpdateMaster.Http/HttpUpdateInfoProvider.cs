using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using SnkUpdateMaster.Core;
using SnkUpdateMaster.Core.Parser;
using SnkUpdateMaster.Core.UpdateSource;
using System.Net;

namespace SnkUpdateMaster.Http
{
    public sealed class HttpUpdateInfoProvider(
        HttpClient httpClient,
        IUpdateInfoParser updateInfoParser,
        string updateInfoUrl,
        ILogger<HttpUpdateInfoProvider>? logger = null) : IUpdateInfoProvider
    {
        private readonly HttpClient _httpClient = httpClient;

        private readonly IUpdateInfoParser _updateInfoParser = updateInfoParser;

        private readonly Uri _updateInfoUri = CreateUpdateInfoUri(httpClient, updateInfoUrl);

        private readonly ILogger<HttpUpdateInfoProvider> _logger = logger ?? NullLogger<HttpUpdateInfoProvider>.Instance;

        public async Task<UpdateInfo?> GetLastUpdatesAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Starting to get update info from HTTP at {UpdateInfoUrl}", _updateInfoUri);

            using var response = await _httpClient.GetAsync(
                _updateInfoUri,
                HttpCompletionOption.ResponseHeadersRead,
                cancellationToken);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                _logger.LogWarning("Update info data not found at {UpdateInfoUrl}", _updateInfoUri);
                return null;
            }

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to download update info data. Status code: {StatusCode}", response.StatusCode);
                throw new HttpRequestException($"Can't download update info data. HTTP status: {response.StatusCode}");
            }

            var responseData = await response.Content.ReadAsByteArrayAsync(cancellationToken);
            if (responseData.Length == 0)
            {
                _logger.LogWarning("Update info data is empty at {UpdateInfoUrl}", _updateInfoUri);
                return null;
            }
            var updateInfo = _updateInfoParser.Parse(responseData);
            _logger.LogInformation("Successfully retrieved update info: {@UpdateInfo}", updateInfo);

            return updateInfo;
        }

        private static Uri CreateUpdateInfoUri(HttpClient httpClient, string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentException("Update info URL can not be null or empty.");
            }

            if (Uri.TryCreate(url, UriKind.Absolute, out var absoluteUri))
            {
                return absoluteUri;
            }

            if (httpClient.BaseAddress == null)
            {
                throw new InvalidOperationException("HttpClient.BaseAddress must be set when using a relative update info URL.");
            }

            return new Uri(httpClient.BaseAddress, url);
        }
    }
}
