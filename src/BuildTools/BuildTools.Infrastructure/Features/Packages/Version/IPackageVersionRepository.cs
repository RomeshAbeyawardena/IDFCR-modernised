using BuildTools.Shared.Features.Packages.Version;
using IDFCR.Abstractions.Persistence;

namespace BuildTools.Infrastructure.Features.Packages.Version;

public interface IPackageVersionRepository : IRepository<PackageVersion, Guid>
{
    
}
