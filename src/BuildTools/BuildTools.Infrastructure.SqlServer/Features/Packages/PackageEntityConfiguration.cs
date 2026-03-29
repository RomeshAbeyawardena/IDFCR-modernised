using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BuildTools.Infrastructure.SqlServer.Features.Packages;

public class PackageEntityConfiguration : IEntityTypeConfiguration<PackageEntity>
{
    public void Configure(EntityTypeBuilder<PackageEntity> builder)
    {
        builder.ToTable("Package", "dbo");

        builder.HasKey(e => e.Id)
            .HasName("PK_PackageId");

        builder.Property(e => e.Id)
            .HasColumnName("PackageId")
            .IsRequired();

        builder.Property(e => e.Name)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(e => e.Alias)
            .HasMaxLength(255)
            .IsRequired(false);

        builder.Property(e => e.Namespace)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(e => e.Description)
            .HasMaxLength(2000)
            .IsRequired(false);

        builder.HasAlternateKey(e => new { e.Name, e.Namespace })
            .HasName("UQ_Package");

        builder.HasIndex(e => e.Namespace)
            .HasDatabaseName("IDX_Package_Namespace");
    }
}