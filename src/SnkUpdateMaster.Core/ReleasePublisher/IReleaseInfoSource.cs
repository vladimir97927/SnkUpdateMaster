using SnkUpdateMaster.Core.Common;

namespace SnkUpdateMaster.Core.ReleasePublisher
{
    public interface IReleaseInfoSource
    {
        Task<PagedData<IEnumerable<ReleaseInfo>>> GetReleaseInfosPagedAsync(int? page = null, int? pageSize = null);
    }
}
