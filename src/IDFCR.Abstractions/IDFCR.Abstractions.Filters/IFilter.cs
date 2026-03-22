using IDFCR.Abstractions.Results;
using System.Data.Entity;
using System.Linq.Expressions;

namespace IDFCR.Abstractions.Filters;

internal class DefaultFilterFactory(IEnumerable<IFilter> filters) : IFilterFactory
{
    public IEnumerable<IFilter<TRequest, TDb>> GetFilters<TRequest, TDb>(IQueryable<TDb> queryable)
    {
        return filters.OfType<IFilter<TRequest, TDb>>();
    }

    public IEnumerable<IPagedFilter<TRequest, TDb>> GetPagedFilters<TRequest, TDb>(IQueryable<TDb> queryable) where TRequest : IPagedQuery
    {
        return filters.OfType<IPagedFilter<TRequest, TDb>>();
    }
}

public interface IFilterFactory
{
    IEnumerable<IFilter<TRequest, TDb>> GetFilters<TRequest, TDb>(IQueryable<TDb> queryable);
    IEnumerable<IPagedFilter<TRequest, TDb>> GetPagedFilters<TRequest, TDb>(IQueryable<TDb> queryable)
        where TRequest : IPagedQuery;
}

public interface IFilter
{

}

public interface IFilter<TRequest, TDb> : IFilter
{
    IQueryable<TDb> Apply(TRequest request);
}

public interface IPagedFilter<TRequest, TDb> : IFilter<TRequest, TDb>
    where TRequest : IPagedQuery
{
    new (IQueryable<TDb> query, int totalEntries) Apply(TRequest request);
}

public abstract class FilterBase<TRequest, TDb>(IQueryable<TDb> queryable) : IFilter<TRequest, TDb>
{
    protected LinqKit.ExpressionStarter<TDb> StarterExpression => LinqKit.PredicateBuilder.New<TDb>(x => true);
    protected IQueryable<TDb> Queryable { get; } = queryable;
    public abstract IQueryable<TDb> Apply(TRequest request);
}

public abstract class PagedFilterBase<TRequest, TDb>(IQueryable<TDb> queryable) : FilterBase<TRequest, TDb>(queryable), IPagedFilter<TRequest, TDb>
    where TRequest : IPagedQuery
{
    protected (IQueryable<TDb> query, int totalEntries) Apply_protected(TRequest request)
    {
        var expression = StarterExpression;
        expression = BuildPredicate(expression);

        var query = Queryable.Where(expression);
        var totalEntries = query.Count();
        var pageIndex = request.PageIndex ?? 0;
        var pageSize = request.PageSize ?? DefaultSize;

        var skip = pageIndex * pageSize;

        query = query.Skip(skip).Take(pageSize);

        return (query, totalEntries);
    }

    protected virtual int DefaultSize { get; } = 1000;
    protected abstract Expression<Func<TDb, bool>> BuildPredicate(LinqKit.ExpressionStarter<TDb> builder);
    
    public override IQueryable<TDb> Apply(TRequest request)
    {
        var (query, _) = Apply_protected(request);

        return query;
    }

    (IQueryable<TDb> query, int totalEntries) IPagedFilter<TRequest, TDb>.Apply(TRequest request)
    {
        return Apply_protected(request);
    }
}