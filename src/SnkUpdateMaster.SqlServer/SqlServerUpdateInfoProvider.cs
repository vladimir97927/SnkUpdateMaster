using Dapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using SnkUpdateMaster.Core;
using SnkUpdateMaster.Core.UpdateSource;
using SnkUpdateMaster.SqlServer.Database;

namespace SnkUpdateMaster.SqlServer
{
    /// <summary>
    /// Класс предоставляет реализацию источника обновлений, работающую с Microsoft SQL Server. 
    /// Используется для получения информации о последнем доступном обновлении из базы данных.
    /// </summary>
    /// <param name="sqlConnectionFactory">Фабрика для создания подключений к SQL Server</param>
    /// <param name="logger">Логгер для регистрации операций поставщика информации об обновлениях</param>
    public class SqlServerUpdateInfoProvider(
        ISqlConnectionFactory sqlConnectionFactory,
        ILogger<SqlServerUpdateInfoProvider>? logger = null) : IUpdateInfoProvider
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory = sqlConnectionFactory;

        private readonly ILogger<SqlServerUpdateInfoProvider> _logger = logger ?? NullLogger<SqlServerUpdateInfoProvider>.Instance;

        /// <summary>
        /// Возвращает информацию о последнем опубликованном обновлении из таблицы UpdateInfo.
        /// </summary>
        /// <returns>Объект с данными обновления <see cref="UpdateInfo"/> или null, если обновления отсутствуют</returns>
        public async Task<UpdateInfo?> GetLastUpdatesAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Requesting latest update info from SQL Server.");
            var connection = _sqlConnectionFactory.GetOpenConnection();

            const string sql =
                "SELECT TOP(1) " +
                "u.[Id], " +
                "u.[Version], " +
                "u.[FileName], " +
                "u.[Checksum], " +
                "u.[ReleaseDate], " +
                "u.[FileDir] " +
                "FROM [dbo].[UpdateInfo] u " +
                "ORDER BY u.[ReleaseDate] DESC";

            var command = new CommandDefinition(sql, cancellationToken: cancellationToken);

            var updateInfo = await connection.QueryFirstOrDefaultAsync<UpdateInfo>(command);

            if (updateInfo == null)
            {
                _logger.LogInformation("No updates found in SQL Server UpdateInfo table.");
            }
            else
            {
                _logger.LogInformation("Latest update loaded from SQL Server. Id: {UpdateId}, Version: {Version}", updateInfo.Id, updateInfo.Version);
            }

            return updateInfo;
        }
    }
}
