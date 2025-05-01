using Dapper;
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
    public class SqlServerUpdateSource(ISqlConnectionFactory sqlConnectionFactory) : IUpdateSource
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory = sqlConnectionFactory;

        /// <summary>
        /// Возвращает информацию о последнем опубликованном обновлении из таблицы AppUpdates.
        /// </summary>
        /// <returns>Объект с данными обновления <see cref="UpdateInfo"/> или null, если обновления отсутствуют</returns>
        public async Task<UpdateInfo?> GetLastUpdatesAsync()
        {
            var connection = _sqlConnectionFactory.GetOpenConnection();
            var updateInfo = await connection.QueryFirstOrDefaultAsync<UpdateInfo>(
                "SELECT TOP(1) " +
                "u.[Id], " +
                "u.[Version], " +
                "u.[FileName], " +
                "u.[Checksum], " +
                "u.[ReleaseDate] " +
                "FROM [dbo].[AppUpdates] u " +
                "ORDER BY u.[ReleaseDate] DESC");

            return updateInfo;
        }
    }
}
