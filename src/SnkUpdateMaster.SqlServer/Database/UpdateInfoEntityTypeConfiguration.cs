using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SnkUpdateMaster.Core;

namespace SnkUpdateMaster.SqlServer.Database
{
    internal class UpdateInfoEntityTypeConfiguration : IEntityTypeConfiguration<UpdateInfo>
    {
        public void Configure(EntityTypeBuilder<UpdateInfo> builder)
        {
            builder.ToTable("AppUpdates");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).HasColumnName("Id");
            builder.Property(x => x.FileName).HasColumnName("FileName");
            builder.Property(x => x.Checksum).HasColumnName("Checksum");
            builder.Property(x => x.ReleaseDate).HasColumnName("ReleaseDate");
            builder.Property(x => x.Version).HasColumnName("Version").HasConversion(
                x => x.ToString(),
                x => new Version(x));
        }
    }
}
