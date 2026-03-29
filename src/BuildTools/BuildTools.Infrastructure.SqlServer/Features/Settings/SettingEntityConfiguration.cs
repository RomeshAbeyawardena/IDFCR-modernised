using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BuildTools.Infrastructure.SqlServer.Features.Settings;

public class SettingEntityConfiguration : IEntityTypeConfiguration<SettingEntity>
{
    public void Configure(EntityTypeBuilder<SettingEntity> builder)
    {
        builder.ToTable("Setting", "SYSTEM_CONFIG");

        builder.HasKey(e => e.Id)
            .HasName("PK_SettingId");

        builder.Property(e => e.Id)
            .HasColumnName("SettingId")
            .HasDefaultValueSql("NEWSEQUENTIALID()", "DF_SettingId")
            .IsRequired();

        builder.Property(e => e.Type)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(e => e.Key)
            .HasMaxLength(200)
            .IsRequired();

        builder.HasIndex(e => e.Key)
            .IsUnique()
            .HasDatabaseName("UQ_SystemConfig_Setting_Key");

        builder.Property(e => e.Value)
            .HasColumnType("nvarchar(max)")
            .IsRequired(false);

        builder.Property(e => e.CreatedTimestampUtc)
            .HasColumnName("CreatedTimestampUtc")
            .HasColumnType("DATETIMEOFFSET(7)")
            .IsRequired();

        builder.Property(e => e.ModifiedTimestampUtc)
            .HasColumnName("LastUpdatedTimestampUtc")
            .HasColumnType("DATETIMEOFFSET(7)")
            .HasDefaultValueSql("GETUTCDATE()", "DF_SystemConfig_Setting_LastUpdatedTimestampUtc")
            .IsRequired();
    }
}
