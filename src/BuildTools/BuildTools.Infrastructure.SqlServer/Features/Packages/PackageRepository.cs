using BuildTools.Infrastructure.Features.Packages;
using BuildTools.Shared.Features.Packages;
using IDFCR.Abstractions.Filters;
using IDFCR.Abstractions.Interceptors;
using IDFCR.Abstractions.Results;
using Microsoft.EntityFrameworkCore;

namespace BuildTools.Infrastructure.SqlServer.Features.Packages;

[RegisteredRepository]
public class PackageRepository(PackageManagerDbContext db, IFilterFactory filterFactory, IEntityInterceptorFactory entityInterceptorFactory) 
    : EntityFrameworkRepositoryBase<PackageManagerDbContext, IPackage, PackageEntity, Package, Guid>(
        db, filterFactory, entityInterceptorFactory), IPackageRepository
{
    public async Task<IUnitResult<Package>> GetPackageAsync(string? name, string? @namespace, CancellationToken cancellationToken)
    {
        var query = FilterFactory.Apply(DbSet.AsNoTracking(), new GetPagedPackagesQuery
        {
            Name = name, 
            Namespace = @namespace
        });

        var result = await query.FirstOrDefaultAsync(cancellationToken);

        if (result is null)
        {
            return UnitResult.NotFound<Package>(name ?? @namespace ?? "Unknown");
        }

        return UnitResult.FromResult(base.Map(result));
    }

    protected override bool IsHandled(Exception exception)
    {
        return true;
    }
}
