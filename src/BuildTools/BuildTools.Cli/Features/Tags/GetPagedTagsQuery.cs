using IDFCR.Abstractions.Results;

namespace BuildTools.Cli.Features.Tags;

public record GetPagedTagsQuery : PagedQuery
{
    public string? Name { get; init; }
    public string? NameContains { get; init; }
}
