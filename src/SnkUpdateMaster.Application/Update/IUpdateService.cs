namespace SnkUpdateMaster.Application.Update
{
    public interface IUpdateService
    {
        Task<bool> CheckForUpdatesAsync();

        Task DownloadUpdatesAsync();
    }
}
