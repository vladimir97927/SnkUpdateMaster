using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using SnkUpdateMaster.Core;
using SnkUpdateMaster.Core.Downloader;
using System.Security;

namespace SnkUpdateMaster.FileSystem
{
    public sealed class FileSystemUpdateDownloader(
        string downloadsDir,
        ILogger<FileSystemUpdateDownloader>? logger = null) : IUpdateDownloader
    {
        private readonly string _downloadsDir = downloadsDir;

        private readonly ILogger<FileSystemUpdateDownloader> _logger = logger ?? NullLogger<FileSystemUpdateDownloader>.Instance;

        public async Task<string> DownloadUpdateAsync(
            UpdateInfo updateInfo,
            IProgress<double>? progress = null,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Starting download of update {UpdateId} version {Version}",
                updateInfo.Id, updateInfo.Version);

            var updateFilePath = Path.Combine(updateInfo.FileDir ?? string.Empty, updateInfo.FileName);
            if (!File.Exists(updateFilePath))
            {
                _logger.LogError("Update file not found at path {UpdatePath}", updateFilePath);
                throw new FileNotFoundException($"Update file not found {updateFilePath}");
            }

            _logger.LogInformation("Copy update file from path {UpdatePath} to directory {DownloadsDir}", updateFilePath, _downloadsDir);

            Directory.CreateDirectory(_downloadsDir);

            var destinationFilePath = Path.Combine(_downloadsDir, updateInfo.FileName);

            try
            {
                await FileSystemStreamHelper.CopyToFileAsync(
                    updateFilePath,
                    destinationFilePath,
                    progress,
                    cancellationToken);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Download of update {UpdateId} was canceled", updateInfo.Id);
                if (File.Exists(destinationFilePath))
                {
                    File.Delete(destinationFilePath);
                    _logger.LogInformation("Deleted incomplete download file at path {DestinationPath}", destinationFilePath);
                }
                throw;
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex, "Access denied when downloading update {UpdateId} to path {DestinationPath}", updateInfo.Id, destinationFilePath);
                throw;
            }
            catch (IOException ex)
            {
                _logger.LogError(ex, "I/O error occurred when downloading update {UpdateId} to path {DestinationPath}", updateInfo.Id, destinationFilePath);
                throw;
            }
            catch (SecurityException ex)
            {
                _logger.LogError(ex, "Security error occurred when downloading update {UpdateId} to path {DestinationPath}", updateInfo.Id, destinationFilePath);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred when downloading update {UpdateId} to path {DestinationPath}", updateInfo.Id, destinationFilePath);
                throw;
            }

            _logger.LogInformation("Completed download of update {UpdateId} to path {DestinationPath}", updateInfo.Id, destinationFilePath);

            return destinationFilePath;
        }
    }
}
