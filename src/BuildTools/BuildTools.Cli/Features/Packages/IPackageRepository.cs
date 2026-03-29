using BuildTools.Shared.Features.Packages;
using IDFCR.Abstractions.Persistence;
using IDFCR.Abstractions.Results;

namespace BuildTools.Cli.Features.Packages;

public interface IPackageRepository : IRepository<Package, Guid>
{
    Task<IUnitResult<Package>> GetPackageAsync(string? name, string? @namespace, CancellationToken cancellationToken);
}
