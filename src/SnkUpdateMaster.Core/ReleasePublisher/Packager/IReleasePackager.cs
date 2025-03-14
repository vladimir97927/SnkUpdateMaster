namespace SnkUpdateMaster.Core.ReleasePublisher.Packager
{
    public interface IReleasePackager
    {
        Task<Release> PackReleaseAsync(string filesDir, Version version);
    }
}
