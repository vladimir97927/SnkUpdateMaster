namespace SnkUpdateMaster.Core.ReleasePublisher.Packager
{
    public interface IReleasePackager
    {
        Task<Release> PackAsync(string sourceDir, string destDir, Version version);
    }
}
