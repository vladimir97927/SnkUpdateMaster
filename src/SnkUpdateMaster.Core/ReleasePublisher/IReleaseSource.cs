namespace SnkUpdateMaster.Core.ReleasePublisher
{
    public interface IReleaseSource
    {
        Task UploadReleaseAsync(Release release);

        Task<Release?> GetReleaseAsync(int id);

        Task RemoveReleaseAsync(Release release);
    }
}
