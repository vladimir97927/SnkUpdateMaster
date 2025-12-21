using FluentFTP.Exceptions;
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
    public class FtpUpdateInfoProvider(
        IAsyncFtpClientFactory ftpClientFactory,
        IUpdateInfoFileParser updateInfoFileParser,
        string updateInfoFilePath) : IUpdateInfoProvider
    {
        private readonly IAsyncFtpClientFactory _ftpClientFactory = ftpClientFactory;

        private readonly IUpdateInfoFileParser _updateInfoFileParser = updateInfoFileParser;

        private readonly string _updateInfoFilePath = updateInfoFilePath;


        /// <summary>
        /// Асинхронно получает информацию о последнем обновлении из файла на FTP-сервере.
        /// </summary>
        /// <returns>Объект с данными обновления <see cref="UpdateInfo"/> или null, если обновления отсутствуют.</returns>
        /// <exception cref="FtpException">Не удалось скачать файл с данными об обновлении с FTP-сервера.</exception>
        public async Task<UpdateInfo?> GetLastUpdatesAsync()
        {
            var client = await _ftpClientFactory.GetConnectClientAsync();
            if (!await client.FileExists(_updateInfoFilePath))
            {
                return null;
            }
            try
            {
                byte[] fileBytes;
                using var stream = new MemoryStream();
                bool isSuccess = await client.DownloadStream(stream, _updateInfoFilePath);
                if (!isSuccess)
                {
                    throw new FtpException($"Can't download update info file from {_updateInfoFilePath}");
                }
                fileBytes = stream.ToArray();
                var updateInfo = _updateInfoFileParser.Parse(fileBytes);
                return updateInfo;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
