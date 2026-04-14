using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BuildTools.Infrastructure.SqlServer.Features.Packages;

public class PackageTagEntityConfiguration : IEntityTypeConfiguration<PackageTagEntity>
{
    public void Configure(EntityTypeBuilder<PackageTagEntity> builder)
    {
        builder.ToTable("PackageTag", "dbo");

        builder.HasKey(e => e.Id)
            .HasName("PK_PackageTag");

        builder.Property(e => e.Id)
            .HasColumnName("PackageTagId")
            .HasDefaultValueSql("NEWSEQUENTIALID()", "DF_PackageTag_PackageTagId")
            .IsRequired();

        builder.Property(e => e.PackageId)
            .IsRequired();

        builder.Property(e => e.TagId)
            .IsRequired();

        builder.HasAlternateKey(e => new { e.PackageId, e.TagId })
            .HasName("UQ_PackageTag");

        builder.HasOne(e => e.Package)
            .WithMany(e => e.PackageTags)
            .HasForeignKey(e => e.PackageId)
            .HasConstraintName("FK_PackageTag_Package")
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(e => e.Tag)
            .WithMany()
            .HasForeignKey(e => e.TagId)
            .HasConstraintName("FK_PackageTag_Tag")
            .OnDelete(DeleteBehavior.NoAction);
    }
}