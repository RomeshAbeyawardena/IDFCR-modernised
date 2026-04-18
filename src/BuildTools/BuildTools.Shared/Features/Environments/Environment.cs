using IDFCR.Abstractions.Mapper;

namespace BuildTools.Shared.Features.Environments;

public class Environment : MapperBase<IEnvironment>, IEnvironment
{
    public object? Id { get; set; }
    public string ExternalReference { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? DisplayName { get; set; }

    public override void Map(IEnvironment source)
    {
        Id = source.Id;
        ExternalReference = source.ExternalReference;
        Name = source.Name;
        DisplayName = source.DisplayName;
    }
}