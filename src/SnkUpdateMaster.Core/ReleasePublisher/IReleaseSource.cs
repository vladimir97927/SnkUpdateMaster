namespace SnkUpdateMaster.Core.ReleasePublisher
{
    public interface IReleaseSource
    {
        Task UploadReleaseAsync(Release release);

        Task<Release> GetReleaseAsync(int id);

        Task DeleteReleaseAsync(Release release);
    }
}
