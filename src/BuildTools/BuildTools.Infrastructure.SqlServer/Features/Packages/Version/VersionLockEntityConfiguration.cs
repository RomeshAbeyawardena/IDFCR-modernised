using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BuildTools.Infrastructure.SqlServer.Features.Packages.Version;

public class VersionLockEntityConfiguration : IEntityTypeConfiguration<VersionLockEntity>
{
    public void Configure(EntityTypeBuilder<VersionLockEntity> builder)
    {
        builder.ToTable("VersionLock");

        builder.HasKey(e => e.Id)
            .HasName("PK_VersionLockId");

        builder.Property(e => e.Id)
            .HasColumnName("VersionLockId")
            .HasDefaultValueSql("NEWSEQUENTIALID()", "DF_VersionLockId")
            .IsRequired();

        builder.Property(e => e.PackageId)
            .IsRequired();

        builder.HasOne<PackageEntity>()
            .WithMany()
            .HasForeignKey(e => e.PackageId)
            .HasConstraintName("FK_VersionLock_Package")
            .OnDelete(DeleteBehavior.NoAction);

        builder.Property(e => e.Reference)
            .HasMaxLength(255)
            .IsRequired(false);

        builder.Property(e => e.VersionPrefix)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(e => e.RevisionId)
            .IsRequired(false);

        builder.Property(e => e.LastRequestedTimestampUtc)
            .HasColumnType("DATETIMEOFFSET(7)")
            .IsRequired();

        builder.Property(e => e.LockedFromTimestampUtc)
            .HasColumnType("DATETIMEOFFSET(7)")
            .IsRequired(false);

        builder.Property(e => e.LockedUntilTimestampUtc)
            .HasColumnType("DATETIMEOFFSET(7)")
            .IsRequired(false);

        builder.Property(e => e.LockReleasedTimestampUtc)
            .HasColumnType("DATETIMEOFFSET(7)")
            .IsRequired(false);

        builder.HasIndex(e => new { e.PackageId, e.VersionPrefix })
            .IsUnique()
            .HasDatabaseName("UQ_VersionLock_PackageId_VersionPrefix");
    }
}
