using IDFCR.Abstractions.Results;

namespace BuildTools.Infrastructure.Features.Environments;

public record GetPagedEnvironmentQuery : PagedQuery, IEnvironmentQuery
{
    public string? Name { get; init; }
    public string? NameContains { get; init; }
    public string? ExternalReference { get; init; }
}
