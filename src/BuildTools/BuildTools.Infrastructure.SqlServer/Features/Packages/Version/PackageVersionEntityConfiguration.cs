using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BuildTools.Infrastructure.SqlServer.Features.Packages.Version;

public class PackageVersionEntityConfiguration : IEntityTypeConfiguration<PackageVersionEntity>
{
    public void Configure(EntityTypeBuilder<PackageVersionEntity> builder)
    {
        builder.ToTable("PackageVersion");

        builder.HasKey(e => e.Id)
            .HasName("PK_PackageVersionId");

        builder.Property(e => e.Id)
            .HasColumnName("PackageVersionId")
            .HasDefaultValueSql("NEWSEQUENTIALID()", "DF_PackageVersionId")
            .IsRequired();

        builder.Property(e => e.PackageId)
            .IsRequired();

        builder.Property(e => e.VersionPrefix)
            .HasColumnName("VersionSuffix")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(e => e.RevisionNumber)
            .IsRequired();

        builder.Property(e => e.ReleaseDateTimestampUtc)
            .HasColumnType("DATETIMEOFFSET(7)")
            .HasDefaultValueSql("GETUTCDATE()")
            .IsRequired();

        builder.Property(e => e.CommitId)
            .HasMaxLength(80)
            .IsRequired();

        builder.Property(e => e.PublishedToFeed)
            .IsRequired(true);

        builder.Property(e => e.LastErrorOnPublishAttemptTimestampUtc)
            .HasColumnName("LastErrorOnPublishAttempt")
            .HasColumnType("DATETIMEOFFSET(7)")
            .IsRequired(false);

        builder.Property(e => e.PublishedTimestampUtc)
            .HasColumnType("DATETIMEOFFSET(7)")
            .IsRequired(false);

        builder.HasIndex(e => new { e.PackageId, e.CommitId })
            .HasDatabaseName("IX_PackageVersion_PackageId_CommitId");

        builder.HasIndex(e => new { e.PackageId, e.VersionPrefix, e.RevisionNumber })
            .IsUnique()
            .HasDatabaseName("UQ_PackageVersion");

        builder.HasOne(e => e.Package)
            .WithMany()
            .HasForeignKey(e => e.PackageId)
            .HasConstraintName("FK_PackageVersion_Package");
    }
}
