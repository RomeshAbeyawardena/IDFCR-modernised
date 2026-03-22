namespace IDFCR.Abstractions.Filters.Tests;

public class GenericTestFilter<TRequest, TDb> : FilterBase<TRequest, TDb>
{
    public override IQueryable<TDb> Apply(IQueryable<TDb> queryable, TRequest request)
    {
        throw new NotImplementedException();
    }
}
