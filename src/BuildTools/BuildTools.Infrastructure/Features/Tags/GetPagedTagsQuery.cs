using IDFCR.Abstractions.Results;

namespace BuildTools.Infrastructure.Features.Tags;

public record GetPagedTagsQuery : PagedSortedQuery
{
    public GetPagedTagsQuery()
    {
        SupportedFields.AddRange("Name", "DisplayName");
    }
    public string? Name { get; init; }
    public string? NameContains { get; init; }
}
