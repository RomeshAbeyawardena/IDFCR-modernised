using IDFCR.Abstractions.Mediator;

namespace BuildTools.Shared.Contracts.Feature.Tags;

public record GetTagsQuery : IUnitResultCollectionRequest<TagDto>
{
    public IEnumerable<string> Names { get; set; } = [];
}
