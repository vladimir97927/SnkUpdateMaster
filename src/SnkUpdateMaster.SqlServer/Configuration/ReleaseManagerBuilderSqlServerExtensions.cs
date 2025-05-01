using SnkUpdateMaster.Core.ReleasePublisher;
using SnkUpdateMaster.SqlServer.Database;

namespace SnkUpdateMaster.SqlServer.Configuration
{
    /// <summary>
    /// Класс предоставляет extension-методы для интеграции SQL Server в <see cref="ReleaseManagerBuilder"/>. 
    /// Упрощает настройку SQL-хранилища релизов.
    /// </summary>
    public static class ReleaseManagerBuilderSqlServerExtensions
    {
        /// <summary>
        /// Регистрирует SQL Server источник релизов
        /// </summary>
        /// <param name="builder">Текущий экземпляр билдера</param>
        /// <param name="connectionString">Строка подключения к SQL Server</param>
        /// <returns>Тот же экземпляр билдера для цепочки вызовов</returns>
        public static ReleaseManagerBuilder WithSqlServerReleaseSource(
            this ReleaseManagerBuilder builder,
            string connectionString)
        {
            var factory = new SqlServerReleaseSourceFactory(connectionString);
            var releaseSource = factory.Create();
            builder.AddDependency(releaseSource);

            var sqlConnectionFactory = new SqlConnectionFactory(connectionString);
            var releaseInfoSource = new SqlServerReleaseInfoSource(sqlConnectionFactory);
            builder.AddDependency(releaseInfoSource);

            return builder;
        }
    }
}
