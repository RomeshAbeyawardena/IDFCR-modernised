using NUnit.Framework;

namespace IDFCR.Abstractions.Filters.Tests;

public class TestFilter
{

}

[TestFixture]
public class FilterFactoryTests
{
    private DefaultFilterFactory _factory;
    [SetUp]
    public void SetUp()
    {
        _factory = new();
    }

    [Test]
    public void Test()
    {

    }
}
