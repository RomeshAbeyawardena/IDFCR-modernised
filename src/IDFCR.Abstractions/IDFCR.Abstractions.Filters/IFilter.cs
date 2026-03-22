namespace IDFCR.Abstractions.Filters;

public interface IFilter
{
}

public interface IFilter<TRequest, TDb> : IFilter
{
    bool CanFilter(TRequest request);
    IQueryable<TDb> Apply(IQueryable<TDb> queryable, TRequest request);
}
