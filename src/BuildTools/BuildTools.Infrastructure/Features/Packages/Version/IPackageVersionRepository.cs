using BuildTools.Shared.Features.Packages.Version;
using IDFCR.Abstractions.Persistence;
using IDFCR.Abstractions.Results;

namespace BuildTools.Infrastructure.Features.Packages.Version;

public interface IPackageVersionRepository : IRepository<PackageVersion, Guid>
{
    Task<IUnitResult<PackageVersion>> GetLatestVersionAsync(object? packageId, string prefix, CancellationToken cancellationToken);
}
