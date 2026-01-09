using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using SnkUpdateMaster.Core;
using SnkUpdateMaster.Core.Parser;
using SnkUpdateMaster.Core.UpdateSource;

namespace SnkUpdateMaster.Http
{
    public sealed class HttpUpdateInfoProvider(
        IHttpClientFactory httpClientFactory,
        IUpdateInfoParser updateInfoParser,
        string updateInfoUrl,
        ILogger<HttpUpdateInfoProvider>? logger = null) : IUpdateInfoProvider
    {
        private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;

        private readonly IUpdateInfoParser _updateInfoParser = updateInfoParser;

        private readonly string _updateInfoUrl = updateInfoUrl;

        private readonly ILogger<HttpUpdateInfoProvider> _logger = logger ?? NullLogger<HttpUpdateInfoProvider>.Instance;

        public Task<UpdateInfo?> GetLastUpdatesAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
