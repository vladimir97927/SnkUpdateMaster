using FluentFTP;
using FluentFTP.Exceptions;
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
    public class FtpUpdateDownloader(
        IAsyncFtpClientFactory ftpClientFactory,
        string downloadsDir) : IUpdateDownloader
    {
        private readonly IAsyncFtpClientFactory _ftpClientFactory = ftpClientFactory;

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
            var client = await _ftpClientFactory.GetConnectClientAsync(cancellationToken);

            var remoteFilePath = Path
                .Combine(updateInfo.FileDir ?? string.Empty, updateInfo.FileName)
                .Replace("\\", "/");
            if (!await client.FileExists(remoteFilePath, cancellationToken))
            {
                throw new FileNotFoundException($"Update file not found {remoteFilePath}");
            }
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
                throw new FtpException($"Can't download updates. FTP status: {status}");
            }

            return localFilePath;
        }
    }
}
