using Microsoft.EntityFrameworkCore;
using SnkUpdateMaster.Core;
using SnkUpdateMaster.SqlServer.Database;

namespace SnkUpdateMaster.SqlServer
{
    internal class SqlServerUpdateSource : IUpdateSource
    {

        public SqlServerUpdateSource()
        {

        }

        public async Task<UpdateInfo?> CheckForUpdatesAsync()
        {

        }
    }
}
