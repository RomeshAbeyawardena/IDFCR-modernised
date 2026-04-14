using BuildTools.Shared.Features.Tags;
using IDFCR.Abstractions.Mapper;

namespace BuildTools.Shared.Features.Packages;

/// <inheritdoc cref="IPackage" />
public class Package : MapperBase<IPackage>, IPackage
{
    /// <inheritdoc/>
    public string Name { get; set; } = null!;
    /// <inheritdoc/>
    public string? Alias { get; set; } = null!;
    /// <inheritdoc/>
    public string Namespace { get; set; } = null!;
    /// <inheritdoc/>
    public string? Description { get; set; } = null!;
    public object? Id { get; set; }

    public IEnumerable<ITag> Tags { get; set; } = [];

    /// <inheritdoc/>
    public override void Map(IPackage source)
    {
        Id = source.Id;
        Name = source.Name;
        Alias = source.Alias;
        Namespace = source.Namespace;
        Description = source.Description;
        Tags = source.Tags;
    }
}
