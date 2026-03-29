using BuildTools.Shared.Features.Packages;
using IDFCR.Abstractions.Mapper;
using IDFCR.Abstractions.Metadata;

namespace BuildTools.Infrastructure.SqlServer.Features.Packages;

public class PackageEntity : MapperBase<IPackage>, IPackage, IIdentifiable<Guid>
{ 
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    /// <inheritdoc/>
    public string? Alias { get; set; } = null!;
    /// <inheritdoc/>
    public string Namespace { get; set; } = null!;
    /// <inheritdoc/>
    public string? Description { get; set; } = null!;
    object? IPackage.Id => Id;

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
