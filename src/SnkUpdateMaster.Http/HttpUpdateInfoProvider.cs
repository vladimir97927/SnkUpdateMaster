using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using SnkUpdateMaster.Core;
using SnkUpdateMaster.Core.Parser;
using SnkUpdateMaster.Core.UpdateSource;
using System.Net;

namespace SnkUpdateMaster.Http
{
    /// <summary>
    /// Provides update information by retrieving and parsing data from a specified HTTP endpoint.
    /// </summary>
    /// <remarks>This provider is typically used to fetch update metadata from a remote server. The update
    /// information is parsed using the specified parser. If a relative URL is supplied, the HttpClient's BaseAddress
    /// must be set. This class is thread-safe for concurrent use.</remarks>
    /// <param name="httpClient">The HTTP client instance used to send requests to the update information endpoint. Must have a valid BaseAddress
    /// if a relative URL is provided.</param>
    /// <param name="updateInfoParser">The parser used to convert the downloaded update information data into an UpdateInfo object.</param>
    /// <param name="updateInfoUrl">The absolute or relative URL of the update information resource. Cannot be null or empty.</param>
    /// <param name="logger">The logger used to record diagnostic messages for this provider. If null, a no-op logger is used.</param>
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

        /// <summary>
        /// Asynchronously retrieves the most recent update information from the configured HTTP endpoint.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="UpdateInfo"/>
        /// object with the latest update information, or <see langword="null"/> if no update information is available.</returns>
        /// <exception cref="HttpRequestException">Thrown if the HTTP request to retrieve update information fails with a non-success status code other than
        /// 404 (Not Found).</exception>
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
