using IDFCR.Abstractions.Mediator;

namespace BuildTools.Shared.Contracts.Features.Tags;

public record GetTagsQuery : IUnitResultCollectionRequest<TagDto>
{
    public IEnumerable<string> Names { get; init; } = [];
}
