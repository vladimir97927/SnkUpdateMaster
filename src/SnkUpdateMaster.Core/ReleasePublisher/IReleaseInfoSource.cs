namespace SnkUpdateMaster.Core.ReleasePublisher
{
    public interface IReleaseInfoSource
    {
        Task<IEnumerable<ReleaseInfo>> GetReleaseInfoPagedAsync(int page, int pageSize);
    }
}
