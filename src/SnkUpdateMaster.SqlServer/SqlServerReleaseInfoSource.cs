using Dapper;
using SnkUpdateMaster.Core.ReleasePublisher;
using SnkUpdateMaster.SqlServer.Database;
using SnkUpdateMaster.SqlServer.Pagination;

namespace SnkUpdateMaster.SqlServer
{
    public class SqlServerReleaseInfoSource(ISqlConnectionFactory sqlConnectionFactory) : IReleaseInfoSource
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory = sqlConnectionFactory;

        public async Task<IEnumerable<ReleaseInfo>> GetReleaseInfosPagedAsync(int? page = null, int? pageSize = null)
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

            return await connection.QueryAsync<ReleaseInfo>(sql, new { pageData.Offset, pageData.Next });
        }
    }
}
