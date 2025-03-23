using Dapper;
using SnkUpdateMaster.Core;
using SnkUpdateMaster.Core.UpdateSource;
using SnkUpdateMaster.SqlServer.Database;

namespace SnkUpdateMaster.SqlServer
{
    public class SqlServerUpdateSource(ISqlConnectionFactory sqlConnectionFactory) : IUpdateSource
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory = sqlConnectionFactory;

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
