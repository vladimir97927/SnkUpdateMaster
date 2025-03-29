namespace SnkUpdateMaster.Core.VersionManager
{
    public interface ICurrentVersionManager
    {
        Task<Version?> GetCurrentVersionAsync();

        Task UpdateCurrentVersionAsync(Version version);
    }
}
