using SnkUpdateMaster.Core;
using SnkUpdateMaster.Core.UpdateSource;

namespace SnkUpdateMaster.Http
{
    public sealed class HttpUpdateInfoProvider : IUpdateInfoProvider
    {
        public Task<UpdateInfo?> GetLastUpdatesAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
