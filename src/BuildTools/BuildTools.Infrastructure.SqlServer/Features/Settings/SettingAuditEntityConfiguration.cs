using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BuildTools.Infrastructure.SqlServer.Features.Settings;

public class SettingAuditEntityConfiguration : IEntityTypeConfiguration<SettingAuditEntity>
{
    public void Configure(EntityTypeBuilder<SettingAuditEntity> builder)
    {
        builder.ToTable("SettingAudit", "SYSTEM_CONFIG");

        builder.HasKey(e => e.Id)
            .HasName("PK_SettingAuditId");

        builder.Property(e => e.Id)
            .HasColumnName("SettingAuditId")
            .HasDefaultValueSql("NEWSEQUENTIALID()", "DF_SettingAuditId")
            .IsRequired();

        builder.Property(e => e.SettingId)
            .HasColumnName("SettingId")
            .IsRequired(false);

        builder.HasIndex(e => e.SettingId)
            .HasDatabaseName("IX_SystemConfig_SettingAudit_SettingId");

        builder.Property(e => e.ChangeDescription)
            .HasColumnType("nvarchar(max)")
            .IsRequired(false);

        builder.Property(e => e.OldValueJson)
            .HasColumnType("nvarchar(max)")
            .IsRequired(false);

        builder.Property(e => e.NewValueJson)
            .HasColumnType("nvarchar(max)")
            .IsRequired(false);

        builder.HasOne<SettingEntity>()
            .WithMany()
            .HasForeignKey(e => e.SettingId)
            .HasConstraintName("FK_SystemConfig_SettingAudit_Setting")
            .OnDelete(DeleteBehavior.SetNull);
    }
}