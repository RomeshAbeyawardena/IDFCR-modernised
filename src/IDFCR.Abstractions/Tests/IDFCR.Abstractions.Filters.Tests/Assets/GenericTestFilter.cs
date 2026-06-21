using IDFCR.Abstractions.Filters.Extensions;

namespace IDFCR.Abstractions.Filters.Tests.Assets;

[GlobalFilter]
public class GenericTestFilter<TRequest, TDb> : FilterBase<TRequest, TDb>
{
    public override IQueryable<TDb> Apply(IQueryable<TDb> queryable, TRequest request)
    {
        return queryable;
    }
}
