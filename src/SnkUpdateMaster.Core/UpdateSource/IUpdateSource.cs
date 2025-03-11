namespace SnkUpdateMaster.Core.UpdateSource
{
    public interface IUpdateSource
    {
        Task<UpdateInfo?> GetLastUpdatesAsync();
    }
}
