using BuildTools.Shared.Features.Packages;
using BuildTools.Shared.Features.Tags;
using IDFCR.Abstractions.Mapper;
using IDFCR.Abstractions.Metadata;

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
