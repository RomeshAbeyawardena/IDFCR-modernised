using IDFCR.Abstractions.Results;

namespace BuildTools.Cli.Features.Packages;

public record GetPagedPackagesQuery : PagedQuery
{
    public string? Name { get; init; }
    public string? Namespace { get; init; }
}
