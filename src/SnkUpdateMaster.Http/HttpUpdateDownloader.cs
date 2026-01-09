using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using SnkUpdateMaster.Core;
using SnkUpdateMaster.Core.Downloader;
using SnkUpdateMaster.FileSystem;

namespace SnkUpdateMaster.Http
{
    public sealed class HttpUpdateDownloader(
        HttpClient httpClient,
        string downloadsDir,
        ILogger<HttpUpdateDownloader>? logger = null) : IUpdateDownloader
    {
        private readonly HttpClient _httpClient = httpClient;

        private readonly string _downloadsDir = downloadsDir;

        private readonly ILogger<HttpUpdateDownloader> _logger = logger ?? NullLogger<HttpUpdateDownloader>.Instance;

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
