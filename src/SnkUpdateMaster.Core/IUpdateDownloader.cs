namespace SnkUpdateMaster.Core
{
    public interface IUpdateDownloader
    {
        Task DownloadUpdateAsync(UpdateInfo updateInfo, string tempPath, IProgress<double> progress, CancellationToken cancellationToken);
    }
}
