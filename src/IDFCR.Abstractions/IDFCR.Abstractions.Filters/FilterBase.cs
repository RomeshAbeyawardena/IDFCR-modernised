namespace IDFCR.Abstractions.Filters;

/// <summary>
/// Base class for filters that always opt in by default.
/// </summary>
/// <typeparam name="TRequest">The request type.</typeparam>
/// <typeparam name="TDb">The queryable element type.</typeparam>
public abstract class FilterBase<TRequest, TDb> : IFilter<TRequest, TDb>
{
    /// <summary>
    /// Gets a new LinqKit predicate starter for composing expressions.
    /// </summary>
    protected LinqKit.ExpressionStarter<TDb> StarterExpression => LinqKit.PredicateBuilder.New<TDb>();

    /// <inheritdoc />
    public virtual bool CanFilter(TRequest request)
    {
        return true;
    }

    /// <inheritdoc />
    public abstract IQueryable<TDb> Apply(IQueryable<TDb> queryable, TRequest request);
}
