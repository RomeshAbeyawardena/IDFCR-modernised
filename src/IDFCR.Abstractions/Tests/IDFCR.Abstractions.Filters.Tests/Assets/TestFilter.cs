namespace IDFCR.Abstractions.Filters.Tests.Assets;

public class TestFilter : FilterBase<TestFilterRequest, Customer>
{
    public override IQueryable<Customer> Apply(IQueryable<Customer> queryable, TestFilterRequest request)
    {
        throw new NotImplementedException();
    }
}
