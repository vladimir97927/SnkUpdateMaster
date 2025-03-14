namespace SnkUpdateMaster.Core.UpdateSource
{
    public interface IUpdateSource
    {
        Task<UpdateInfo?> GetLastUpdatesAsync();

        Task<UpdateInfo?> GetUpdateAsync(int id);

        Task AddUpdateAsync(UpdateInfo updateInfo);

        void RemoveUpdate(UpdateInfo updateInfo);
    }
}
