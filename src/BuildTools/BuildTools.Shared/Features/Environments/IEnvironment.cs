using IDFCR.Abstractions.Mapper;

namespace BuildTools.Shared.Features.Environments;

public interface IEnvironment : IMapper<IEnvironment>
{
    object? Id { get; }
    string ExternalReference { get; }
    string Name { get; }
    string? DisplayName { get; }
}