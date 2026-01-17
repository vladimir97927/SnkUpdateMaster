using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using SnkUpdateMaster.Core;
using SnkUpdateMaster.Core.Downloader;
using SnkUpdateMaster.FileSystem;

namespace SnkUpdateMaster.Http
{
    /// <summary>
    /// Provides functionality to download update files over HTTP and save them to a specified local directory.
    /// </summary>
    /// <remarks>This class is thread-safe for concurrent download operations. Download progress can be
    /// tracked via the provided progress reporting mechanism in download methods. Ensure that the specified downloads
    /// directory has appropriate write permissions.</remarks>
    /// <param name="httpClient">The HTTP client instance used to perform download requests. Must have a valid BaseAddress set if relative update
    /// paths are used.</param>
    /// <param name="downloadsDir">The path to the local directory where downloaded update files will be saved. The directory will be created if it
    /// does not exist.</param>
    /// <param name="logger">The logger used to record informational and error messages during the download process. If null, a no-op logger
    /// is used.</param>
    public sealed class HttpUpdateDownloader(
        HttpClient httpClient,
        string downloadsDir,
        ILogger<HttpUpdateDownloader>? logger = null) : IUpdateDownloader
    {
        private readonly HttpClient _httpClient = httpClient;

        private readonly string _downloadsDir = downloadsDir;

        private readonly ILogger<HttpUpdateDownloader> _logger = logger ?? NullLogger<HttpUpdateDownloader>.Instance;

        /// <summary>
        /// Downloads the update package specified by the provided update information and saves it to the local
        /// downloads directory asynchronously.
        /// </summary>
        /// <remarks>If the download is canceled or fails, any partially downloaded file is deleted. The
        /// method creates the downloads directory if it does not exist.</remarks>
        /// <param name="updateInfo">An object containing information about the update to download, including its identifier, version, and file
        /// name. Cannot be null.</param>
        /// <param name="progress">An optional progress reporter that receives the download progress as a value between 0.0 and 1.0. If null,
        /// progress is not reported.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the download operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the full path to the downloaded
        /// update file.</returns>
        /// <exception cref="HttpRequestException">Thrown if the update file cannot be downloaded due to an HTTP error.</exception>
        public async Task<string> DownloadUpdateAsync(
            UpdateInfo updateInfo,
            IProgress<double>? progress = null,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Starting download of update {UpdateId} version {Version}",
                updateInfo.Id, updateInfo.Version);

            var downloadUri = ResolveDownloadUri(updateInfo);
            Directory.CreateDirectory(_downloadsDir);
            var localFilePath = Path.Combine(_downloadsDir, updateInfo.FileName);
            try
            {
                using var response = await _httpClient.GetAsync(
                    downloadUri,
                    HttpCompletionOption.ResponseHeadersRead,
                    cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Failed to download update file. Status code: {StatusCode}", response.StatusCode);
                    throw new HttpRequestException($"Can't download update file. HTTP status: {response.StatusCode}");
                }

                await using var contentStream = await response.Content.ReadAsStreamAsync(cancellationToken);
                await using var fileStream = new FileStream(
                    localFilePath,
                    FileMode.Create,
                    FileAccess.Write,
                    FileShare.None,
                    bufferSize: 81920,
                    useAsync: true);

                await FileStreamHelper.CopyToAsync(
                    contentStream,
                    fileStream,
                    response.Content.Headers.ContentLength ?? -1,
                    progress,
                    cancellationToken);

                _logger.LogInformation("Successfully downloaded update {UpdateId} to {LocalPath}",
                    updateInfo.Id, localFilePath);

                return localFilePath;
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Download of update {UpdateId} was canceled", updateInfo.Id);
                DeleteLocalFile(localFilePath);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to download update {UpdateId} from {DownloadUri}", updateInfo.Id, downloadUri);
                DeleteLocalFile(localFilePath);
                throw;
            }
        }

        private Uri ResolveDownloadUri(UpdateInfo updateInfo)
        {
            if (string.IsNullOrWhiteSpace(updateInfo.FileName))
            {
                throw new InvalidOperationException("Update info file name can not be null or emplty");
            }

            if (!string.IsNullOrWhiteSpace(updateInfo.FileDir))
            {
                if (Uri.TryCreate(updateInfo.FileDir, UriKind.Absolute, out var absoluteUri))
                {
                    return absoluteUri;
                }

                if (_httpClient.BaseAddress == null)
                {
                    throw new InvalidOperationException("Base address must be set when using relative update paths.");
                }

                var relativePath = $"{updateInfo.FileDir.TrimEnd('/')}/{updateInfo.FileName}";

                return new Uri(_httpClient.BaseAddress, relativePath);
            }

            if (_httpClient.BaseAddress == null)
            {
                throw new InvalidOperationException("Base address must be set when update info does not provide a file directory.");
            }

            return new Uri(_httpClient.BaseAddress, updateInfo.FileName);
        }

        private static void DeleteLocalFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }
}
