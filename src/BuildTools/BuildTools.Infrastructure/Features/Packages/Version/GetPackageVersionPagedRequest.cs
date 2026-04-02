using IDFCR.Abstractions.Results;

namespace BuildTools.Infrastructure.Features.Packages.Version;

public record GetPackageVersionPagedRequest : PagedQuery
{
    public string? PackageVersion { get; set; }
    public string? PackageName { get; set; }
}
