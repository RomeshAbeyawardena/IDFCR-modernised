using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BuildTools.Infrastructure.SqlServer.Features.Outbox;

public class OutboxEntityConfiguration : IEntityTypeConfiguration<OutboxEntity>
{
    public void Configure(EntityTypeBuilder<OutboxEntity> builder)
    {
        builder.ToTable("Outbox", "dbo");

        builder.HasKey(e => e.Id)
            .HasName("PK_OutboxId");

        builder.Property(e => e.Id)
            .HasColumnName("OutboxId")
            .HasDefaultValueSql("NEWSEQUENTIALID()", "DF_OutboxId")
            .IsRequired();

        builder.Property(e => e.Data)
            .HasColumnType("nvarchar(max)")
            .IsRequired(false);

        builder.Property(e => e.CompletedTimestampUtc)
            .HasColumnType("DATETIMEOFFSET(7)")
            .IsRequired(false);

        builder.Property(e => e.FailedTimestampUtc)
            .HasColumnType("DATETIMEOFFSET(7)")
            .IsRequired(false);

        builder.Property(e => e.AcknowledgedTimestampUtc)
            .HasColumnType("DATETIMEOFFSET(7)")
            .IsRequired(false);

        builder.Property(e => e.CreatedTimestampUtc)
            .HasColumnType("DATETIMEOFFSET(7)")
            .HasDefaultValueSql("GETUTCDATE()", "DF_Outbox_CreatedTimestampUtc")
            .IsRequired();

        builder.Property(e => e.ModifiedTimestampUtc)
            .HasColumnName("LastUpdatedTimestampUtc")
            .HasColumnType("DATETIMEOFFSET(7)")
            .IsRequired(false);

        builder.HasIndex(e => e.CompletedTimestampUtc)
            .HasDatabaseName("IX_Outbox_CompletedTimestampUtc");

        builder.HasIndex(e => e.FailedTimestampUtc)
            .HasDatabaseName("IX_Outbox_FailedTimestampUtc");

        builder.HasIndex(e => e.AcknowledgedTimestampUtc)
            .HasDatabaseName("IX_Outbox_AcknowledgedTimestampUtc");
    }
}