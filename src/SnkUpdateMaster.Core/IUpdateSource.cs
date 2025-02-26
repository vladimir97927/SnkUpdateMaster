namespace SnkUpdateMaster.Core
{
    public interface IUpdateSource
    {
        Task<UpdateInfo?> GetLastUpdatesAsync();
    }
}
