using Dapper;
using SnkUpdateMaster.Core.Common;
using SnkUpdateMaster.Core.ReleasePublisher;
using SnkUpdateMaster.SqlServer.Database;
using SnkUpdateMaster.SqlServer.Pagination;

namespace SnkUpdateMaster.SqlServer
{
    /// <summary>
    /// Класс предоставляет пагинированный список информации о релизах из Microsoft SQL Server. 
    /// Используется для получения основных метаданных обновлений без загрузки бинарных данных.
    /// </summary>
    /// <param name="sqlConnectionFactory">Фабрика подключений к SQL Server</param>
    public class SqlServerReleaseInfoSource(ISqlConnectionFactory sqlConnectionFactory) : IReleaseInfoSource
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory = sqlConnectionFactory;

        /// <summary>
        /// Постраничное получение информации о загруженных релизах
        /// </summary>
        /// <param name="page">Номер страницы</param>
        /// <param name="pageSize">Количество элементов на странице</param>
        /// <returns>Объект <see cref="PageData"/> со списком объектов <see cref="ReleaseInfo"/></returns>
        public async Task<PagedData<IEnumerable<ReleaseInfo>>> GetReleaseInfosPagedAsync(int? page = null, int? pageSize = null)
        {
            var connection = _sqlConnectionFactory.GetOpenConnection();
            var sql = "SELECT " +
                "u.[Id], " +
                "u.[Version], " +
                "u.[ReleaseDate] " +
                "FROM [dbo].[AppUpdates] u " +
                "ORDER BY u.[ReleaseDate] DESC";
            var pageData = PagedQueryHelper.GetPageData(page, pageSize);
            sql = PagedQueryHelper.AppendPageStatement(sql);
            var releaseInfos = await connection.QueryAsync<ReleaseInfo>(sql, new { pageData.Offset, pageData.Next });

            var totalCount = await connection.QuerySingleOrDefaultAsync<int>(
                "SELECT COUNT(*) " +
                "FROM [dbo].[AppUpdates]");

            return new PagedData<IEnumerable<ReleaseInfo>>(releaseInfos, page, pageSize, totalCount);
        }
    }
}
