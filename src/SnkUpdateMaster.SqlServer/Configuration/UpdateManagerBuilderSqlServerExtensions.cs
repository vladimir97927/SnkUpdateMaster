using SnkUpdateMaster.Core;
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
        /// Регистрирует SQL Server источник обновлений
        /// </summary>
        /// <param name="builder">Текущий экземпляр билдера</param>
        /// <param name="connectionString">Строка подключения к SQL Server</param>
        /// <param name="downloadsDir">Директория для хранения загруженных файлов</param>
        /// <returns>Тот же экземпляр билдера для цепочки вызовов</returns>
        public static UpdateManagerBuilder WithSqlServerUpdateProvider(
            this UpdateManagerBuilder builder,
            string connectionString,
            string downloadsDir)
        {
            var sqlConnectionFactory = new SqlConnectionFactory(connectionString);
            var updateSource = new SqlServerUpdateSource(sqlConnectionFactory);
            var downloader = new SqlServerUpdateDownloader(sqlConnectionFactory, downloadsDir);

            builder.AddDependency<IUpdateSource>(updateSource);
            builder.AddDependency<IUpdateDownloader>(downloader);

            return builder;
        }
    }
}
