namespace IDFCR.Abstractions.Filters;

public abstract class FilterBase<TRequest, TDb> : IFilter<TRequest, TDb>
{
    protected LinqKit.ExpressionStarter<TDb> StarterExpression => LinqKit.PredicateBuilder.New<TDb>();
    public virtual bool CanFilter(TRequest request)
    {
        return true;
    }

    public abstract IQueryable<TDb> Apply(IQueryable<TDb> queryable, TRequest request);
}
