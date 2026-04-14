using BuildTools.Infrastructure.SqlServer.Features.Tags;
using BuildTools.Shared.Features.Packages;
using BuildTools.Shared.Features.Tags;
using IDFCR.Abstractions.Mapper;
using IDFCR.Abstractions.Metadata;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations.Schema;

namespace BuildTools.Infrastructure.SqlServer.Features.Packages;

public class PackageEntity : MapperBase<IPackage>, IPackage, IIdentifiable<Guid>
{
    IEnumerable<ITag> IPackage.Tags => PackageTags?.Select(pt => pt.Tag!) ?? [];
    object? IPackage.Id => Id;

    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    /// <inheritdoc/>
    public string? Alias { get; set; } = null!;
    /// <inheritdoc/>
    public string Namespace { get; set; } = null!;
    /// <inheritdoc/>
    public string? Description { get; set; } = null!;

    public virtual ICollection<PackageTagEntity> PackageTags { get; set; } = [];

    /// <inheritdoc/>
    public override void Map(IPackage source)
    {
        if (source.Id is not null && source.Id is Guid id)
        {
            Id = id;
        }
        Name = source.Name;
        Alias = source.Alias;
        Namespace = source.Namespace;
        Description = source.Description;
    }
}

public class PackageTagEntity
{
    public Guid Id { get; set; }
    public Guid PackageId { get; set; }
    public Guid TagId { get; set; }

    public virtual PackageEntity? Package { get; set; }
    public virtual TagEntity? Tag { get; set; }
}
        
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