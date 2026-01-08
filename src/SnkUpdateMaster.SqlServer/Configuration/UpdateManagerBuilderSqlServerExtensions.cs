using Microsoft.Extensions.Logging;
using SnkUpdateMaster.Core;
using SnkUpdateMaster.Core.Common;
using SnkUpdateMaster.Core.Downloader;
using SnkUpdateMaster.Core.UpdateSource;
using SnkUpdateMaster.SqlServer.Database;

namespace SnkUpdateMaster.SqlServer.Configuration
{
    /// <summary>
    /// Класс предоставляет extension-метод для интеграции SQL Server 
    /// провайдера обновлений в <see cref="UpdateManagerBuilder"/>.
    /// Реализует полчение информации и загрузку обновлений через SQL Server.
    /// </summary>
    public static class UpdateManagerBuilderSqlServerExtensions
    {
        /// <summary>
        /// Регистрирует SQL Server как источник информации об обновлениях
        /// </summary>
        /// <param name="builder">Текущий экземпляр билдера</param>
        /// <param name="sqlConnectionFactory">Фабрика SQL подключения</param>
        /// <returns>Тот же экземпляр билдера для цепочки вызовов</returns>
        public static UpdateManagerBuilder WithSqlServerUpdateInfoProvider(
            this UpdateManagerBuilder builder,
            ISqlConnectionFactory sqlConnectionFactory)
        {
            IUpdateInfoProvider UpdateInfoProviderFactory(IDependencyResolver dr)
            {
                var loggerFactory = dr.Resolve<ILoggerFactory>();
                var logger = loggerFactory?.CreateLogger<SqlServerUpdateInfoProvider>();
                return new SqlServerUpdateInfoProvider(sqlConnectionFactory, logger);
            }

            builder.RegisterFactory(UpdateInfoProviderFactory);
            return builder;
        }


        /// <summary>
        /// Регистрирует SQL Server загрузчик обновлений
        /// </summary>
        /// <param name="builder">Текущий экземпляр билдера</param>
        /// <param name="sqlConnectionFactory">Фабрика SQL подключения</param>
        /// <param name="downloadsDir">Директория для хранения загруженных файлов</param>
        /// <returns>Тот же экземпляр билдера для цепочки вызовов</returns>
        public static UpdateManagerBuilder WithSqlServerUpdateDownloader(
            this UpdateManagerBuilder builder,
            ISqlConnectionFactory sqlConnectionFactory,
            string downloadsDir)
        {
            IUpdateDownloader UpdateDownloaderFactory(IDependencyResolver dr)
            {
                var loggerFactory = dr.Resolve<ILoggerFactory>();
                var logger = loggerFactory?.CreateLogger<SqlServerUpdateDownloader>();
                return new SqlServerUpdateDownloader(sqlConnectionFactory, downloadsDir, logger);
            }

            builder.RegisterFactory(UpdateDownloaderFactory);
            return builder;
        }
    }
}
