namespace SnkUpdateMaster.Core.ReleasePublisher
{
    public interface IReleaseInfoSource
    {
        Task<IEnumerable<ReleaseInfo>> GetReleaseInfosPagedAsync(int? page = null, int? pageSize = null);
    }
}
