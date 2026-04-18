using IDFCR.Abstractions.Results;
using System.Linq.Expressions;

namespace IDFCR.Abstractions.Filters;

/// <summary>
/// Base class for paged filters that derive their predicate from the request.
/// </summary>
/// <typeparam name="TRequest">The request type.</typeparam>
/// <typeparam name="TDb">The queryable element type.</typeparam>
public abstract class PagedFilterBase<TRequest, TDb> : FilterBase<TRequest, TDb>, IPagedFilter<TRequest, TDb>
    where TRequest : IPagedQuery
{
    /// <summary>
    /// Applies the derived predicate and returns the filtered query.
    /// </summary>
    /// <remarks>
    /// The returned total row count is left as zero here; paging totals are calculated by <see cref="PagedFilter.ApplyPaging{TDb, TRequest}(IQueryable{TDb}, TRequest, int, Func{IQueryable{TDb}, int}?)"/>.
    /// </remarks>
    /// <param name="queryable">The source query.</param>
    /// <param name="request">The request used to build the predicate.</param>
    /// <returns>The filtered query and a placeholder row count.</returns>
    protected (IQueryable<TDb> query, int totalEntries) Apply_protected(IQueryable<TDb> queryable, TRequest request)
    {
        var expression = BuildPredicate(queryable, request);

        var query = queryable.Where(expression);

        var totalEntries = 0;

        return (query, totalEntries);
    }

    /// <summary>
    /// Gets the default page size used by derived implementations.
    /// </summary>
    protected virtual int DefaultSize { get; } = 500;

    /// <summary>
    /// Builds the predicate used to filter the query.
    /// </summary>
    /// <param name="queryable">The source query.</param>
    /// <param name="request">The request used to build the predicate.</param>
    /// <returns>The predicate to apply to the query.</returns>
    protected abstract Expression<Func<TDb, bool>> BuildPredicate(IQueryable<TDb> queryable, TRequest request);

    /// <inheritdoc />
    public override IQueryable<TDb> Apply(IQueryable<TDb> queryable, TRequest request)
    {
        var (query, _) = Apply_protected(queryable, request);

        return query;
    }

    /// <inheritdoc />
    (IQueryable<TDb> query, int totalEntries) IPagedFilter<TRequest, TDb>.Apply(IQueryable<TDb> queryable, TRequest request)
    {
        return Apply_protected(queryable, request);
    }
}
