namespace SnkUpdateMaster.Application.ReleasePackages
{
    public interface IReleasePackageService
    {
        Task CreateReleaseAsync(string releasePath, string versionName, int versionCode, CancellationToken cancellationToken);
    }
}
