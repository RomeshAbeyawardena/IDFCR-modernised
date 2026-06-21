using IDFCR.Abstractions.Filters.Extensions;
using IDFCR.Abstractions.Filters.Tests.Assets;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace IDFCR.Abstractions.Filters.Tests;

[TestFixture]
public class FilterFactoryTests
{
    private IFilterFactory _factory;
    private ServiceProvider _serviceProvider;

    [SetUp]
    public void SetUp()
    {
        var services = new ServiceCollection();
        services
            .ScanFilters(typeof(FilterFactoryTests).Assembly)
            .AddGenericFilter(typeof(GenericTestFilter<,>));

        _serviceProvider = services.BuildServiceProvider();
        _factory = _serviceProvider.GetRequiredService<IFilterFactory>();
    }

    [TearDown]
    public void TearDown()
    {
        _serviceProvider?.Dispose();
    }

    [Test]
    public void GetFilters_WithMatchingGenericTypes_ReturnsDirectAndGenericFilters()
    {
        // Act
        var filters = _factory.GetFilters<TestFilterRequest, Customer>().ToArray();

        // Assert
        Assert.That(filters, Has.Length.EqualTo(2), "Should return direct filter + generic filter");
        Assert.That(filters[0], Is.InstanceOf<TestFilter>());
        Assert.That(filters[1], Is.InstanceOf<GenericTestFilter<TestFilterRequest, Customer>>());
    }

    [Test]
    public void GetPagedFilters_WithMatchingGenericTypes_ReturnsDefaultFilter()
    {
        // Act
        var pagedFilters = _factory.GetPagedFilters<TestPagedFilterRequest, Customer>().ToArray();

        // Assert
        Assert.That(pagedFilters, Has.Length.EqualTo(1));
        Assert.That(pagedFilters[0], Is.InstanceOf<DefaultPagedFilter<TestPagedFilterRequest, Customer>>());
    }

    [Test]
    public void DefaultPagedFilter_Apply_FiltersQuerySuccessfully()
    {
        // Arrange
        var defaultPagedFilter = _factory
            .GetPagedFilters<TestPagedFilterRequest, Customer>()
            .Cast<DefaultPagedFilter<TestPagedFilterRequest, Customer>>()
            .First();

        var customers = new List<Customer>
        {
            new(), new(), new(), new(), new(), new()
        };
        var request = new TestPagedFilterRequest { PageIndex = 0, PageSize = 3 };

        // Act
        var query = defaultPagedFilter.ApplyPaging(customers.AsQueryable(), request);

        // Assert
        Assert.That(query, Is.Not.Null);
        // Add specific assertions about the filtered result
        var result = query.ToList();
        Assert.That(result, Has.Count.EqualTo(3));
    }
}
