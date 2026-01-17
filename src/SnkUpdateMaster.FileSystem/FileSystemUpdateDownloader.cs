using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using SnkUpdateMaster.Core;
using SnkUpdateMaster.Core.Downloader;
using System.Security;

namespace SnkUpdateMaster.FileSystem
{
    /// <summary>
    /// Provides functionality to download update files from the file system to a specified directory, supporting
    /// progress reporting and cancellation.
    /// </summary>
    /// <remarks>This class implements the IUpdateDownloader interface using file system operations. It
    /// ensures that update files are copied to the target directory and supports progress reporting and cancellation.
    /// Logging is performed for key events and errors. The class is thread-safe for concurrent download
    /// operations.</remarks>
    /// <param name="downloadsDir">The directory path where downloaded update files will be stored. Must be a valid, writable file system path.</param>
    /// <param name="logger">An optional logger used to record diagnostic and error information during update downloads. If not specified, a
    /// no-op logger is used.</param>
    public sealed class FileSystemUpdateDownloader(
        string downloadsDir,
        ILogger<FileSystemUpdateDownloader>? logger = null) : IUpdateDownloader
    {
        private readonly string _downloadsDir = downloadsDir;

        private readonly ILogger<FileSystemUpdateDownloader> _logger = logger ?? NullLogger<FileSystemUpdateDownloader>.Instance;

        /// <summary>
        /// Asynchronously downloads the specified update file to the local downloads directory.
        /// </summary>
        /// <remarks>If the operation is canceled, any partially downloaded file will be deleted from the
        /// downloads directory. The method will overwrite any existing file with the same name in the downloads
        /// directory.</remarks>
        /// <param name="updateInfo">Information about the update to download, including file name, version, and source directory. Cannot be
        /// null.</param>
        /// <param name="progress">An optional progress reporter that receives the completion percentage of the download operation. If null,
        /// progress is not reported.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the download operation.</param>
        /// <returns>A string containing the full path to the downloaded update file in the local downloads directory.</returns>
        /// <exception cref="FileNotFoundException">Thrown if the update file specified by <paramref name="updateInfo"/> does not exist at the source path.</exception>
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
                await FileStreamHelper.CopyToFileAsync(
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
