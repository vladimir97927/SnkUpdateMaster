﻿using Microsoft.EntityFrameworkCore;
using SnkUpdateMaster.Core;
using SnkUpdateMaster.Core.UpdateSource;
using SnkUpdateMaster.SqlServer.Database;

namespace SnkUpdateMaster.SqlServer
{
    internal class SqlServerUpdateSource(SnkUpdateMasterContext context) : IUpdateSource
    {
        private readonly SnkUpdateMasterContext _context = context;

        public async Task<UpdateInfo?> GetLastUpdatesAsync()
        {
            return await _context.UpdateInfos
                .OrderByDescending(x => x.ReleaseDate)
                .FirstOrDefaultAsync();
        }
    }
}
