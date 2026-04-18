using IDFCR.Abstractions.Mediator;

namespace BuildTools.Shared.Contracts.Feature.Tags;

public record GetTagQuery : IUnitResultRequest<TagDto>
{
    public string? Name { get; init; }
    public string? NameContains { get; init; }
}
