using IDFCR.Abstractions.Mediator;

namespace BuildTools.Shared.Contracts.Features.Tags;

public record GetPagedTagsQuery : PagedUnitResultRequestBase<TagDto>
{
    public string? Name { get; init; }
    public string? NameContains { get; init; }
}

public record GetTagsQuery : IUnitResultCollectionRequest<TagDto>
{
    public IEnumerable<string> Names { get; init; } = [];
}
