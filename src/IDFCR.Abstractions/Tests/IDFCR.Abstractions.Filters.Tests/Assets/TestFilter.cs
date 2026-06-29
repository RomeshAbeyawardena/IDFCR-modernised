namespace IDFCR.Abstractions.Filters.Tests.Assets;

public class TestFilter : FilterBase<ITestFilter, Customer>
{
    public override IQueryable<Customer> Apply(IQueryable<Customer> queryable, ITestFilter request)
    {
        throw new NotImplementedException();
    }
}
