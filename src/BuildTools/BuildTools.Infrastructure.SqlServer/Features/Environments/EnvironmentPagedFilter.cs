using BuildTools.Infrastructure.Features.Environments;
using IDFCR.Abstractions.Filters;
using LinqKit;
using System.Linq.Expressions;

namespace BuildTools.Infrastructure.SqlServer.Features.Environments;

public class EnvironmentFilter : FilterBase<IEnvironmentQuery, EnvironmentEntity>
{
    public static Expression<Func<EnvironmentEntity, bool>> BuildPredicate(Expression<Func<EnvironmentEntity, bool>> predicate, IEnvironmentQuery request)
    {
        var query = predicate;

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

    public override IQueryable<EnvironmentEntity> Apply(IQueryable<EnvironmentEntity> queryable, IEnvironmentQuery request)
    {
        return queryable.Where(BuildPredicate(base.StarterExpression.DefaultExpression = e => true, request));
    }
}

public class EnvironmentPagedFilter : PagedFilterBase<GetPagedEnvironmentQuery, EnvironmentEntity>
{
    
    protected override Expression<Func<EnvironmentEntity, bool>> BuildPredicate(IQueryable<EnvironmentEntity> queryable, GetPagedEnvironmentQuery request)
    {
        return EnvironmentFilter.BuildPredicate(base.StarterExpression.DefaultExpression = e => true, request);
    }
}
