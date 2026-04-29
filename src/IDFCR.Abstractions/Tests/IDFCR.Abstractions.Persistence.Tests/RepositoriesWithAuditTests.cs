using IDFCR.Abstractions.Filters;
using IDFCR.Abstractions.Interceptors;
using IDFCR.Abstractions.Interceptors.Factories;
using IDFCR.Abstractions.Interceptors.Interceptors;
using IDFCR.Abstractions.Interceptors.Processors;
using IDFCR.Abstractions.Interceptors.Providers;
using IDFCR.Abstractions.Persistence.Tests.Assets;
using IDFCR.Abstractions.Results;
using IDFCR.Abstractions.Results.Extensions;
using IDFCR.TestUtilities;
using Moq;
using NUnit.Framework;

namespace IDFCR.Abstractions.Persistence.Tests;

public class CustomerAudit
{

}

public class AuditCustomerProcessor() : AuditProcessorBase<DbCustomer, CustomerAudit>("Customer")
{
    public override async Task<IUnitResult> AuditChangesAsync(DbCustomer oldValue, DbCustomer newValue, CancellationToken cancellationToken)
    {
        Assert.That(Provider, Is.Not.Null);
        Assert.That(Provider.InterceptorFactory, Is.Not.Null);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(Provider.InterceptorFactory.ScopedResources.TryGetScopedResource<DbContextMarker<DbCustomer>>(out var marker), Is.True);
            Assert.That(marker, Is.Not.Null);
            Assert.That(marker, Is.InstanceOf<DbContextMarker<DbCustomer>>());
        }
        
        await Task.CompletedTask;
        return UnitResult.Success(UnitAction.Add);
    }
}


[TestFixture]
public class RepositoriesWithAuditTests
{
    private InternalMemoryMockRepository<ICustomer, DbCustomer, Customer> memoryMockRepository;
    private DefaultEntityInterceptorFactory defaultEntityInterceptorFactory;
    private AuditEntityChangesInterceptor auditEntityChangesInterceptor;
    private DefaultAuditProcessorProvider defaultAuditProcessorProvider;
    private Mock<IFilterFactory> filterFactoryMock;

    [SetUp]
    public void SetUp()
    {
        filterFactoryMock = new();
        defaultAuditProcessorProvider = new([new AuditCustomerProcessor()]);
        auditEntityChangesInterceptor = new(defaultAuditProcessorProvider);
        defaultEntityInterceptorFactory = new([auditEntityChangesInterceptor]);
        memoryMockRepository = new(defaultEntityInterceptorFactory, filterFactoryMock.Object, new DefaultScopedResources());
    }

    [Test]
    public async Task Upsert_PopulatesSharedContext_AccessibleInAuditProcessor()
    {
        var id = (await memoryMockRepository.UpsertAsync(new Customer
        {

        }, CancellationToken.None)).GetResultOrDefault();

        await memoryMockRepository.UpsertAsync(new Customer
        {
            Id = id
        }, CancellationToken.None);
    }
}
