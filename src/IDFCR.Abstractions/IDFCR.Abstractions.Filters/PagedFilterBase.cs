using IDFCR.Abstractions.Results;
using System.Linq.Expressions;

namespace IDFCR.Abstractions.Filters;

public abstract class PagedFilterBase<TRequest, TDb> : FilterBase<TRequest, TDb>, IPagedFilter<TRequest, TDb>
    where TRequest : IPagedQuery
{
    protected (IQueryable<TDb> query, int totalEntries) Apply_protected(IQueryable<TDb> queryable, TRequest request)
    {
        var expression = StarterExpression;
        expression = BuildPredicate(queryable, expression);

        var query = queryable.Where(expression);

        var totalEntries = 0;

        return (query, totalEntries);
    }

    protected virtual int DefaultSize { get; } = 500;
    protected abstract Expression<Func<TDb, bool>> BuildPredicate(IQueryable<TDb> queryable, LinqKit.ExpressionStarter<TDb> builder);
    
    public override IQueryable<TDb> Apply(IQueryable<TDb> queryable, TRequest request)
    {
        var (query, _) = Apply_protected(queryable, request);
        
        return query;
    }

    (IQueryable<TDb> query, int totalEntries) IPagedFilter<TRequest, TDb>.Apply(IQueryable<TDb> queryable, TRequest request)
    {
        return Apply_protected(queryable, request);
    }
}