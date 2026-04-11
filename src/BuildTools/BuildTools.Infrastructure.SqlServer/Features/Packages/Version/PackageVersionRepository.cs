using BuildTools.Infrastructure.Features.Packages.Version;
using BuildTools.Shared.Features.Packages.Version;
using IDFCR.Abstractions.Filters;
using IDFCR.Abstractions.Interceptors;
using IDFCR.Abstractions.Results;
using IDFCR.Persistence.EntityFrameworkCore;
using IDFCR.Persistence.EntityFrameworkCore.Attributes;
using Microsoft.EntityFrameworkCore;

namespace BuildTools.Infrastructure.SqlServer.Features.Packages.Version;

[RegisteredRepository]
public class PackageVersionRepository(PackageManagerDbContext db, IFilterFactory filterFactory, IEntityInterceptorFactory entityInterceptorFactory)
    : EntityFrameworkRepositoryBase<PackageManagerDbContext, IPackageVersion, PackageVersionEntity, PackageVersion, Guid>(db, filterFactory, entityInterceptorFactory), IPackageVersionRepository
{
    public async Task<IUnitResult<PackageVersion>> GetLatestVersionAsync(object? packageId, CancellationToken cancellationToken)
    {
        if(packageId is Guid id)
        {
            var result = await DbSet.AsNoTracking()
                .Where(x => x.PackageId == id)
                .OrderByDescending(x => x.RevisionNumber) //latest revision number
                .FirstOrDefaultAsync(cancellationToken);

            if (result is null)
            {
                return UnitResult.NotFound<PackageVersion>(packageId);
            }

            return UnitResult.FromResult(Map(result));
        }

        return UnitResult.Failed<PackageVersion>(new InvalidOperationException("A package ID is required"));
    }

    protected override bool IsHandled(Exception exception)
    {
        return true;
    }
}
