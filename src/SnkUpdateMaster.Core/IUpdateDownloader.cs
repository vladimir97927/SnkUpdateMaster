namespace SnkUpdateMaster.Core
{
    public interface IUpdateDownloader
    {
        Task<string> DownloadUpdateAsync(UpdateInfo updateInfo, IProgress<double> progress, CancellationToken cancellationToken);
    }
}
