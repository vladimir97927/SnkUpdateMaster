namespace SnkUpdateMaster.Core.Downloader
{
    public interface IUpdateDownloaderFactory
    {
        IUpdateDownloader Create();
    }
}
