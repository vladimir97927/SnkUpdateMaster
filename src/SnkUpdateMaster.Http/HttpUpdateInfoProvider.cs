using Microsoft.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using SnkUpdateMaster.Core;
using SnkUpdateMaster.Core.Parser;
using SnkUpdateMaster.Core.UpdateSource;

namespace SnkUpdateMaster.Http
{
    public sealed class HttpUpdateInfoProvider(
        IUpdateInfoParser updateInfoParser,
        string updateInfoUrl,
        ILogger<HttpUpdateInfoProvider>? logger = null) : IUpdateInfoProvider
    {

        private readonly IUpdateInfoParser _updateInfoParser = updateInfoParser;

        private readonly string _updateInfoUrl = updateInfoUrl;

        private readonly ILogger<HttpUpdateInfoProvider> _logger = logger ?? NullLogger<HttpUpdateInfoProvider>.Instance;

        public Task<UpdateInfo?> GetLastUpdatesAsync(CancellationToken cancellationToken = default)
        {
            
            
        }
    }
}
