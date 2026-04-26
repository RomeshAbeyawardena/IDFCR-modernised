using IDFCR.Abstractions.Filters;
using IDFCR.Abstractions.Interceptors;
using IDFCR.Abstractions.Interceptors.Interceptors;
using IDFCR.Abstractions.Mapper;
using IDFCR.Abstractions.Metadata;
using IDFCR.Abstractions.Results;
using IDFCR.TestUtilities;
using Moq;
using NUnit.Framework;

namespace IDFCR.Abstractions.Persistence.Tests;

public interface ICustomer : IMapper<ICustomer>
{

}

public class DbCustomer : MapperBase<ICustomer>, ICustomer, IIdentifiable<Guid>, IAuditable
{
    string IAuditable.AuditEntityName => "Customer";
    public Guid Id { get; set; }
    public override void Map(ICustomer source)
    {
        
    }
}

public class Customer : MapperBase<ICustomer>, ICustomer
{
    public override void Map(ICustomer source)
    {

    }
}

public class CustomerAudit
{

}

public class AuditCustomerProcessor() : AuditProcessorBase<Customer, CustomerAudit>("Customer")
{
    public override async Task<IUnitResult> AuditChangesAsync(Customer oldValue, Customer newValue, CancellationToken cancellationToken)
    {
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
        memoryMockRepository = new(defaultEntityInterceptorFactory, filterFactoryMock.Object);
    }

    [Test]
    public void Test()
    {
        memoryMockRepository.
    }
}
