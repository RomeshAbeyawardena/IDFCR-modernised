using BuildTools.Infrastructure.Features.Packages;
using IDFCR.Abstractions.Filters;
using System.Linq.Expressions;

namespace BuildTools.Infrastructure.SqlServer.Features.Packages;

public class PackagePagedFilter : PagedFilterBase<GetPagedPackagesQuery, PackageEntity>
{
    protected override Expression<Func<PackageEntity, bool>> BuildPredicate(IQueryable<PackageEntity> queryable, GetPagedPackagesQuery request)
    {
        var query = StarterExpression;
        query.DefaultExpression = p => true;

        if (!string.IsNullOrWhiteSpace(request.Namespace))
        {
            query = query.And(p => p.Namespace == request.Namespace);
        }
        else if (!string.IsNullOrWhiteSpace(request.NamespaceContains))
        {
            query = query.And(p => p.Namespace.Contains(request.NamespaceContains));
        }

        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            query = query.And(p => p.Name == request.Name);
        }
        else if (!string.IsNullOrWhiteSpace(request.NameContains))
        {
            query = query.And(p => p.Name.Contains(request.NameContains));
        }

        return query;
    }
}