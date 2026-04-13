using BuildTools.Shared.Features.Environments;
using IDFCR.Abstractions.Mapper;

namespace BuildTools.Cli.Features.Environments;

public class EnvironmentDto : MapperBase<IEnvironment>, IEnvironment
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
