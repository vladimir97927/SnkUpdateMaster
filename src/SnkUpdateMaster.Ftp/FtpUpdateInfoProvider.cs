using FluentFTP.Exceptions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using SnkUpdateMaster.Core;
using SnkUpdateMaster.Core.Files;
using SnkUpdateMaster.Core.UpdateSource;

namespace SnkUpdateMaster.Ftp
{
    /// <summary>
    /// Класс предоставляет реализацию источника обновлений, работающую с FTP-сервером.
    /// Используется для получения информации о последнем доступном обновлении из файла на FTP-сервере.
    /// </summary>
    /// <param name="ftpClientFactory">Фабрика подключений к FTP-серверу.</param>
    /// <param name="updateInfoFileParser">Парсер фала обновлений в класс <see cref="UpdateInfo"/>.</param>
    /// <param name="updateInfoFilePath">Путь к файлу с данными об обнолвении на FTP-сервере.</param>
    /// <param name="logger">Логгер для отслеживания процесса получения информации об обновлениях</param>
    public class FtpUpdateInfoProvider(
        IAsyncFtpClientFactory ftpClientFactory,
        IUpdateInfoFileParser updateInfoFileParser,
        string updateInfoFilePath,
        ILogger? logger = null) : IUpdateInfoProvider
    {
        private readonly IAsyncFtpClientFactory _ftpClientFactory = ftpClientFactory;

        private readonly IUpdateInfoFileParser _updateInfoFileParser = updateInfoFileParser;

        private readonly string _updateInfoFilePath = updateInfoFilePath;

        private readonly ILogger _logger = logger ?? NullLogger.Instance;

        /// <summary>
        /// Асинхронно получает информацию о последнем обновлении из файла на FTP-сервере.
        /// </summary>
        /// <returns>Объект с данными обновления <see cref="UpdateInfo"/> или null, если обновления отсутствуют.</returns>
        /// <exception cref="FtpException">Не удалось скачать файл с данными об обновлении с FTP-сервера.</exception>
        public async Task<UpdateInfo?> GetLastUpdatesAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Starting to get update info from FTP server.");
            var client = await _ftpClientFactory.GetConnectClientAsync(cancellationToken);

            try
            {
                if (!await client.FileExists(_updateInfoFilePath, cancellationToken))
                {
                    _logger.LogWarning("Update info file not found at path: {Path}", _updateInfoFilePath);
                    return null;
                }
                byte[] fileBytes;
                using var stream = new MemoryStream();
                bool isSuccess = await client.DownloadStream(stream, _updateInfoFilePath, token: cancellationToken);
                if (!isSuccess)
                {
                    _logger.LogError("Failed to download update info file from path: {Path}", _updateInfoFilePath);
                    throw new FtpException($"Can't download update info file from {_updateInfoFilePath}");
                }
                fileBytes = stream.ToArray();
                var updateInfo = _updateInfoFileParser.Parse(fileBytes);
                _logger.LogInformation("Successfully retrieved update info: {@UpdateInfo}", updateInfo);

                return updateInfo;
            }
            finally
            {
                _logger.LogDebug("Disposing FTP client used for update info download.");
                await client.DisposeAsync();
            }
        }
    }
}
