using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using SnkUpdateMaster.Core;
using SnkUpdateMaster.Core.Downloader;

namespace SnkUpdateMaster.FileSystem
{
    public sealed class FileSystemUpdateDownloader(
        string downloadsDir,
        ILogger<FileSystemUpdateDownloader>? logger = null) : IUpdateDownloader
    {
        private readonly string _downloadsDir = downloadsDir;

        private readonly ILogger<FileSystemUpdateDownloader> _logger = logger ?? NullLogger<FileSystemUpdateDownloader>.Instance;

        public Task<string> DownloadUpdateAsync(
            UpdateInfo updateInfo,
            IProgress<double>? progress = null,
            CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
