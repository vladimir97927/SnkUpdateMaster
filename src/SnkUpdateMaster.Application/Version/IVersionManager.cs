namespace SnkUpdateMaster.Application.Version
{
    public interface IVersionManager
    {
        Task<int> GetInstalledVersionAsync();

        Task SetInstalledVersionAsync(int version);
    }
}
