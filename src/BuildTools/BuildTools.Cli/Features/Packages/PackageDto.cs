using BuildTools.Shared.Features.Packages;
using BuildTools.Shared.Features.Tags;
using IDFCR.Abstractions.Mapper;

namespace BuildTools.Cli.Features.Packages;

public class PackageDto : MapperBase<IPackage>, IPackage
{
    /// <inheritdoc/>
    public string Name { get; set; } = null!;
    /// <inheritdoc/>
    public string? Alias { get; set; } = null!;
    /// <inheritdoc/>
    public string Namespace { get; set; } = null!;
    /// <inheritdoc/>
    public string? Description { get; set; } = null!;
    public object? Identifier { get; set; }
    public IEnumerable<ITag> Tags { get; set; } = [];

    /// <inheritdoc/>
    public override void Map(IPackage source)
    {
        Identifier = source.Identifier;
        Name = source.Name;
        Alias = source.Alias;
        Namespace = source.Namespace;
        Description = source.Description;
        Tags = source.Tags;
    }
}
