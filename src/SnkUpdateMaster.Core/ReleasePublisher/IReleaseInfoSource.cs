namespace SnkUpdateMaster.Core.ReleasePublisher
{
    public interface IReleaseInfoSource
    {
        Task<IEnumerable<ReleaseInfo>> GetReleaseInfosPagedAsync(int page, int pageSize);
    }
}
