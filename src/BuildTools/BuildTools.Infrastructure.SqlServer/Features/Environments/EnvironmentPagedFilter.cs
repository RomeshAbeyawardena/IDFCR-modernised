using BuildTools.Infrastructure.Features.Environments;
using IDFCR.Abstractions.Filters;
using LinqKit;
using System.Linq.Expressions;

namespace BuildTools.Infrastructure.SqlServer.Features.Environments;

public class EnvironmentPagedFilter : PagedFilterBase<GetPagedEnvironmentQuery, EnvironmentEntity>
{
    internal Expression<Func<EnvironmentEntity, bool>> BuildPredicate(IEnvironmentQuery request)
    {
        var query = base.StarterExpression.DefaultExpression = e => true;

        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            query = query.And(x => x.Name == request.Name);
        }
        else if (!string.IsNullOrWhiteSpace(request.NameContains))
        {
            query = query.And(x => x.Name.Contains(request.NameContains));
        }

        if (!string.IsNullOrWhiteSpace(request.ExternalReference))
        {
            query = query.And(x => x.ExternalReference == request.ExternalReference);
        }

        return query;
    }
    protected override Expression<Func<EnvironmentEntity, bool>> BuildPredicate(IQueryable<EnvironmentEntity> queryable, GetPagedEnvironmentQuery request)
    {
        return BuildPredicate(request);
    }
}
