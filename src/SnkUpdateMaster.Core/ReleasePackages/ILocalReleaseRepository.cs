namespace SnkUpdateMaster.Core.ReleasePackages
{
    public interface ILocalReleaseRepository
    {
        Task<ReleaseInfo?> GetReleaseInfoAsync();

        Task SaveReleaseInfoAsync(ReleaseInfo releaseInfo);
    }
}
