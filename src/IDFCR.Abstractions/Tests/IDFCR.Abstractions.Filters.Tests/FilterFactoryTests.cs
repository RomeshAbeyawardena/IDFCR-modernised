using IDFCR.Abstractions.Filters.Extensions;
using IDFCR.Abstractions.Filters.Tests.Assets;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace IDFCR.Abstractions.Filters.Tests;

[TestFixture]
public class FilterFactoryTests
{
    private IFilterFactory _factory;
    private ServiceCollection _services;
    [SetUp]
    public void SetUp()
    {
        _services = new ServiceCollection();
        _services
            .ScanFilters(typeof(FilterFactoryTests).Assembly)
            .AddGenericFilter(typeof(GenericTestFilter<,>));

        _factory = _services.BuildServiceProvider().GetRequiredService<IFilterFactory>();
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

        var pagedFilters = _factory.GetPagedFilters<TestPagedFilterRequest, Customer>().ToArray();
        Assert.That(pagedFilters, Has.Length.EqualTo(1));

        var defaultPagedFilter = pagedFilters[0];

        Assert.That(defaultPagedFilter, Is.InstanceOf<DefaultPagedFilter<TestPagedFilterRequest, Customer>>());

        List<Customer> customers = [];

        var query = defaultPagedFilter.Apply(customers.AsQueryable(), new TestPagedFilterRequest { PageIndex = 0, PageSize = 200 });
    }
}
