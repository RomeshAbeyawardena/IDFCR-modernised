using IDFCR.Abstractions.Results;
using System.Data.Entity;
using System.Linq.Expressions;

namespace IDFCR.Abstractions.Filters;

public interface IFilter<TRequest, TDb>
{
    IQueryable<TDb> Apply(TRequest request);
}

public interface IPagedFilter<TRequest, TDb> : IFilter<TRequest, TDb>
    where TRequest : IPagedQuery;

public abstract class FilterBase<TRequest, TDb>(IQueryable<TDb> queryable) : IFilter<TRequest, TDb>
{
    protected LinqKit.ExpressionStarter<TDb> StarterExpression => LinqKit.PredicateBuilder.New<TDb>(x => true);
    protected IQueryable<TDb> Queryable { get; } = queryable;
    public abstract IQueryable<TDb> Apply(TRequest request);
}

public abstract class PagedFilterBase<TRequest, TDb>(IQueryable<TDb> queryable) : FilterBase<TRequest, TDb>(queryable), IPagedFilter<TRequest, TDb>
    where TRequest : IPagedQuery
{
    protected virtual int DefaultSize { get; } = 1000;
    protected abstract Expression<Func<TDb, bool>> BuildPredicate(LinqKit.ExpressionStarter<TDb> builder);
    public override IQueryable<TDb> Apply(TRequest request)
    {
        var expression = StarterExpression;
        expression = BuildPredicate(expression);
        
        var query = Queryable.Where(expression);

        var pageIndex = request.PageIndex ?? 0;
        var pageSize = request.PageSize ?? DefaultSize;

        var skip = pageIndex * pageSize;

        query = query.Skip(skip).Take(pageSize);
        
        return query;
    }
}