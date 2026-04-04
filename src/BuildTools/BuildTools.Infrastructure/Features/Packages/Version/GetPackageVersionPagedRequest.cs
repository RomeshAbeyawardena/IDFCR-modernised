using IDFCR.Abstractions.Results;

namespace BuildTools.Infrastructure.Features.Packages.Version;

public record GetPackageVersionPagedRequest : PagedQuery
{
    public string? PackageNamespaceContains { get; set; }
    public string? PackageNamespace { get; set; }
    public string? PackageVersion { get; set; }
    public string? PackageName { get; set; }
    public string? PackageNameContains { get; set; }
}
