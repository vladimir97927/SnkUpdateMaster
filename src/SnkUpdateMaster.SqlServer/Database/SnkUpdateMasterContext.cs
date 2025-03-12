﻿using Microsoft.EntityFrameworkCore;
using SnkUpdateMaster.Core;

namespace SnkUpdateMaster.SqlServer.Database
{
    internal class SnkUpdateMasterContext(DbContextOptions options) : DbContext(options)
    {
        public DbSet<UpdateInfo> UpdateInfos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(SnkUpdateMasterContext).Assembly);
        }
    }
}
