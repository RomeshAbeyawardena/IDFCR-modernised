using BuildTools.Infrastructure.Features.Packages.Version;
using IDFCR.Abstractions.Filters;
using System.Linq.Expressions;

namespace BuildTools.Infrastructure.SqlServer.Features.Packages.Version;

public class PackageVersionFilter : PagedFilterBase<GetPackageVersionPagedRequest, PackageVersionEntity>
{
    protected override Expression<Func<PackageVersionEntity, bool>> BuildPredicate(IQueryable<PackageVersionEntity> queryable, GetPackageVersionPagedRequest request)
    {
        var query = base.StarterExpression;
        query = query.DefaultExpression = e => true;

        if (!string.IsNullOrWhiteSpace(request.PackageVersion))
        {
            query = query.And(e => request.PackageVersion.StartsWith(e.VersionPrefix));
        }

        if (!string.IsNullOrWhiteSpace(request.PackageName))
        {
            query = query.And(e => e.Package.Name == request.PackageName);
        }
        else if (!string.IsNullOrWhiteSpace(request.PackageNameContains))
        {
            query = query.And(e => e.Package.Name.Contains(request.PackageNameContains));
        }

        if (!string.IsNullOrWhiteSpace(request.PackageNamespace))
        {
            query = query.And(e => e.Package.Namespace == request.PackageNamespace);
        }
        else if (!string.IsNullOrWhiteSpace(request.PackageNamespaceContains))
        {
            query = query.And(e => e.Package.Namespace.Contains(request.PackageNamespaceContains));
        }

        return query;
    }
}
