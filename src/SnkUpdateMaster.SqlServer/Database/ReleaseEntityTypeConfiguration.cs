using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SnkUpdateMaster.Core.ReleasePublisher;

namespace SnkUpdateMaster.SqlServer.Database
{
    internal class ReleaseEntityTypeConfiguration : IEntityTypeConfiguration<Release>
    {
        public void Configure(EntityTypeBuilder<Release> builder)
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
            builder.Property(x => x.FileData).HasColumnName("FileData");
        }
    }
}
