using IDFCR.Abstractions.Results;

namespace BuildTools.Infrastructure.Features.Tags;

public record GetPagedTagsQuery : PagedQuery
{
    public string? Name { get; init; }
    public string? NameContains { get; init; }
}
