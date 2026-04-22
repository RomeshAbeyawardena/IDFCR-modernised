using IDFCR.Abstractions.Mediator;
using IDFCR.Abstractions.Metadata;

namespace BuildTools.Shared.Contracts.Features.Tags;

public record GetPagedTagsQuery : PagedUnitResultRequestBase<TagDto>
{
    public IEnumerable<ISort> SortFields { get; init; } = [];
    public string? Name { get; init; }
    public string? NameContains { get; init; }
}
