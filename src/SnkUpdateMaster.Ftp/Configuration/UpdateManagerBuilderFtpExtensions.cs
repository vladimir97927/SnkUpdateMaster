using Microsoft.Extensions.Logging;
using SnkUpdateMaster.Core;
using SnkUpdateMaster.Core.Downloader;
using SnkUpdateMaster.Core.Files;
using SnkUpdateMaster.Core.UpdateSource;

namespace SnkUpdateMaster.Ftp.Configuration
{
    /// <summary>
    /// Предоставляет методы расширения для настройки поставщиков обновлений и загрузчиков на основе FTP.
    /// </summary>
    public static class UpdateManagerBuilderFtpExtensions
    {
        /// <summary>
        /// Использование FTP-поставщика информации об обновлениях для получения метаданных
        /// обновлений.
        /// </summary>
        /// <remarks>Этот метод регистрирует реализацию <see cref="IUpdateInfoProvider"/>, которая
        /// извлекает метаданные обновления с FTP-сервера. Убедитесь, что предоставленная фабрика FTP-клиента и анализатор файлов
        /// совместимы с ожидаемым форматом файла обновления и конфигурацией сервера.</remarks>
        /// <param name="builder">Текущий экземпляр билдера.</param>
        /// <param name="updateInfoFileParser">Парсер, используемый для интерпретации файлов с информацией об обновлениях, полученных с FTP-источника.</param>
        /// <param name="asyncFtpClientFactory">Фабрика для создания асинхронных экземпляров FTP-клиента, используемых для доступа к файлу с информацией об обновлениях.</param>
        /// <param name="updateFileInfoPath">Путь к файлу с информацией об обновлениях на FTP-сервере.</param>
        /// <returns>Тот же экземпляр билдера для цепочки вызовов.</returns>
        public static UpdateManagerBuilder WithFtpUpdateInfoProvider(
            this UpdateManagerBuilder builder,
            IUpdateInfoFileParser updateInfoFileParser,
            IAsyncFtpClientFactory asyncFtpClientFactory,
            string updateFileInfoPath)
        {
            builder.TryGetDependency(out ILogger? sharedLogger);
            var updateSource = new FtpUpdateInfoProvider(asyncFtpClientFactory, updateInfoFileParser, updateFileInfoPath, sharedLogger);

            builder.RegisterInstance<IUpdateInfoProvider>(updateSource);

            return builder;
        }

        /// <summary>
        /// Настраивает менеджер обновлений на использование загрузчика обновлений по FTP.
        /// </summary>
        /// <remarks>Этот метод регистрирует реализацию <see cref="IUpdateDownloader"/>, которая использует FTP
        /// для получения файлов обновлений.</remarks>
        /// <param name="builder">Текущий экземпляр билдера.</param>
        /// <param name="asyncFtpClientFactory">Фабрика для создания асинхронных экземпляров FTP-клиента, используемых для доступа к файлу с информацией об обновлениях.</param>
        /// <param name="downloadsDir">Путь к каталогу, в котором будут храниться загруженные файлы обновлений.</param>
        /// <returns>Тот же экземпляр билдера для цепочки вызовов.</returns>
        public static UpdateManagerBuilder WithFtpUpdateDownloader(
            this UpdateManagerBuilder builder,
            IAsyncFtpClientFactory asyncFtpClientFactory,
            string downloadsDir)
        {
            builder.TryGetDependency(out ILogger? sharedLogger);
            var updateDownloader = new FtpUpdateDownloader(asyncFtpClientFactory, downloadsDir, sharedLogger);

            builder.RegisterInstance<IUpdateDownloader>(updateDownloader);

            return builder;
        }
    }
}
