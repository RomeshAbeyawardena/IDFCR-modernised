using IDFCR.Abstractions.Results;

namespace BuildTools.Infrastructure.Features.Packages;

public record GetPagedPackagesQuery : PagedQuery
{
    public string? Name { get; init; }
    public string? NameContains { get; init; }
    public string? Namespace { get; init; }
    public string? NamespaceContains { get; init; }
}
