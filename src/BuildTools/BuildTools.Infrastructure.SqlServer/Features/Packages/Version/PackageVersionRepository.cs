using BuildTools.Infrastructure.Features.Packages.Version;
using BuildTools.Shared.Features.Packages.Version;
using IDFCR.Abstractions.Filters;
using IDFCR.Abstractions.Interceptors;
using IDFCR.Persistence.EntityFrameworkCore;
using IDFCR.Persistence.EntityFrameworkCore.Attributes;

namespace BuildTools.Infrastructure.SqlServer.Features.Packages.Version;

[RegisteredRepository]
public class PackageVersionRepository(PackageManagerDbContext db, IFilterFactory filterFactory, IEntityInterceptorFactory entityInterceptorFactory)
    : EntityFrameworkRepositoryBase<PackageManagerDbContext, IPackageVersion, PackageVersionEntity, PackageVersion, Guid>(db, filterFactory, entityInterceptorFactory), IPackageVersionRepository
{
    protected override bool IsHandled(Exception exception)
    {
        return true;
    }
}
