using FluentFTP;
using FluentFTP.Exceptions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using SnkUpdateMaster.Core;
using SnkUpdateMaster.Core.Downloader;

namespace SnkUpdateMaster.Ftp
{
    /// <summary>
    /// Класс реализует загрузчик обновлений из FTP-сервера.
    /// Сохраняет файлы обновлений из FTP в указанную директорию с поддержкой
    /// прогресса и отмены.
    /// </summary>
    /// <param name="ftpClientFactory">Фабрика подключений к FTP-серверу.</param>
    /// <param name="downloadsDir">Директория для сохранения файлов.</param>
    /// <param name="logger">Логгер для отслеживания процесса загрузки.</param>
    public class FtpUpdateDownloader(
        IAsyncFtpClientFactory ftpClientFactory,
        string downloadsDir,
        ILogger<FtpUpdateDownloader>? logger = null) : IUpdateDownloader
    {
        private readonly IAsyncFtpClientFactory _ftpClientFactory = ftpClientFactory;

        private readonly ILogger<FtpUpdateDownloader> _logger = logger ?? NullLogger<FtpUpdateDownloader>.Instance;

        private readonly string _downloadsDir = downloadsDir;

        /// <summary>
        /// Скаяивает обновления в указанную директорию с FTP-сервера.
        /// </summary>
        /// <param name="updateInfo">Метаданные запрашиваемого обновления.</param>
        /// <param name="progress">Объект для отслеживания прогресса (0.0-1.0).</param>
        /// <param name="cancellationToken">Токен отмены операции.</param>
        /// <returns>Полный путь к скачанному файлу.</returns>
        /// <exception cref="FileNotFoundException">Файл обновления не найден на сервере.</exception>
        /// <exception cref="FtpException">Не удалось скачать файл обновления с сервера.</exception>
        public async Task<string> DownloadUpdateAsync(
            UpdateInfo updateInfo,
            IProgress<double>? progress = null,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Starting download of update {UpdateId} version {Version}",
                updateInfo.Id, updateInfo.Version);
            var client = await _ftpClientFactory.GetConnectClientAsync(cancellationToken);

            try
            {
                var remoteFilePath = Path
                    .Combine(updateInfo.FileDir ?? string.Empty, updateInfo.FileName)
                    .Replace("\\", "/");
                if (!await client.FileExists(remoteFilePath, cancellationToken))
                {
                    _logger.LogError("Update file not found on FTP server at path {RemotePath}", remoteFilePath);
                    throw new FileNotFoundException($"Update file not found {remoteFilePath}");
                }

                _logger.LogInformation("Downloading update file from FTP path {RemotePath} to directory {DownloadsDir}", remoteFilePath, _downloadsDir);
                var ftpProgress = progress == null
                    ? null
                    : new Progress<FtpProgress>(p =>
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

                Directory.CreateDirectory(_downloadsDir);

                var localFilePath = Path.Combine(_downloadsDir, updateInfo.FileName);
                var status = await client.DownloadFile(
                    localFilePath,
                    remoteFilePath,
                    progress: ftpProgress,
                    token: cancellationToken);

                if (status != FtpStatus.Success)
                {
                    _logger.LogError("Failed to download update file from FTP server. Status: {Status}", status);
                    throw new FtpException($"Can't download updates. FTP status: {status}");
                }

                _logger.LogInformation("Successfully downloaded update {UpdateId} to {LocalPath}",
                    updateInfo.Id, localFilePath);
                return localFilePath;
            }
            finally
            {
                _logger.LogDebug("Disposing FTP client after download attempt for update {UpdateId}", updateInfo.Id);
                await client.DisposeAsync();
            }
        }
    }
}
