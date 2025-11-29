using FluentFTP;
using SnkUpdateMaster.Core;
using SnkUpdateMaster.Core.Downloader;

namespace SnkUpdateMaster.Ftp
{
    public class FtpUpdateDownloader : IUpdateDownloader
    {
        private readonly IAsyncFtpClientFactory _ftpClientFactory;

        private readonly string _downloadsDir;

        public FtpUpdateDownloader(IAsyncFtpClientFactory ftpClientFactory, string downloadsDir)
        {
            _ftpClientFactory = ftpClientFactory;
            _downloadsDir = downloadsDir;
        }

        public async Task<string> DownloadUpdateAsync(
            UpdateInfo updateInfo,
            IProgress<double> progress,
            CancellationToken cancellationToken = default)
        {
            var client = await _ftpClientFactory.GetConnectClientAsync(cancellationToken);

            var remoteFilePath = Path.Combine(updateInfo.FileDir ?? string.Empty, updateInfo.FileName);
            if (!await client.FileExists(remoteFilePath, cancellationToken))
            {
                throw new FileNotFoundException($"Update file not found {remoteFilePath}");
            }
            var ftpProgress = new Progress<FtpProgress>(p =>
            {
                if (p.Progress < 0)
                {
                    progress.Report(0);
                }
                else
                {
                    progress.Report(p.Progress / 100.0);
                }
            });
            
            var localFilePath = Path.Combine(_downloadsDir, updateInfo.FileName);
            var status = await client.DownloadFile(
                localFilePath,
                remoteFilePath,
                progress: ftpProgress,
                token: cancellationToken);

            if (status != FtpStatus.Success)
            {
                throw new Exception($"Can't download updates. FTP status: {status}");
            }

            return localFilePath;
        }
    }
}
