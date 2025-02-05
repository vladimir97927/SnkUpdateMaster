namespace SnkUpdateMaster.Core.ReleasePackages
{
    public interface IRemoteReleaseRepository
    {
        Task AddReleasePackageAsync(ReleasePackage releasePackage);

        Task<ReleaseInfo?> GetLatestReleaseInfoAsync();

        Task<ReleasePackage?> GetLatestReleasePackage();
    }
}
