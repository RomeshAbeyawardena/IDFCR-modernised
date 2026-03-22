using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace IDFCR.Abstractions.Filters.Tests;

[TestFixture]
public class FilterFactoryTests
{
    private DefaultFilterFactory _factory;
    private ServiceCollection _services;
    [SetUp]
    public void SetUp()
    {
        _services = new ServiceCollection();
        _services.AddTransient(typeof(IFilter<,>), typeof(GenericTestFilter<,>));
        _factory = new([new TestFilter()], _services.BuildServiceProvider());
    }

    [Test]
    public void GetFilters_WithMatchingGenericTypes_ReturnsBothDirectAndServiceProviderFilters()
    {
        // Act
        var filters = _factory.GetFilters<TestFilterRequest, Customer>().ToArray();

        // Assert
        Assert.That(filters, Has.Length.EqualTo(2));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(filters[0], Is.InstanceOf<TestFilter>());
            Assert.That(filters[1], Is.InstanceOf<GenericTestFilter<TestFilterRequest, Customer>>());
        }
    }
}
