using BuildTools.Cli.Features.Tags;
using BuildTools.Cli.ManagedStreams;
using BuildTools.Infrastructure.Features.Tags;
using BuildTools.Infrastructure.SqlServer.Features.Tags;
using BuildTools.Shared.Features.Tags;
using IDFCR.Abstractions.Filters;
using IDFCR.Abstractions.Interceptors;
using IDFCR.Abstractions.Results;
using IDFCR.TestUtilities;
using Moq;

namespace BuildTools.Cli.Tests;

public class InternalMemoryTagRepository(IEntityInterceptorFactory entityInterceptorFactory, IFilterFactory filterFactory)
    : InternalMemoryMockRepository<ITag, TagEntity, Tag>(entityInterceptorFactory, filterFactory), ITagRepository
{
    public Task<IUnitResult> AddTagsAsync(IEnumerable<Tag> tags, CancellationToken cancellationToken)
    {
        
    }

    public Task<IUnitResultCollection<Tag>> GetExistingTagsAsync(IEnumerable<string> tags, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<IUnitResult<Tag>> GetTagAsync(string name, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

public class TagWriteOperationTests
{
    private TagWriteOperation sut;

    private Mock<IServiceProvider> serviceProviderMock;
    private Mock<IManagedStream> managedStreamMock;
    private InternalMemoryTagRepository internalMemoryTagRepository;
    private Mock<IEntityInterceptorFactory> entityInterceptorFactoryMock;
    private Mock<IFilterFactory> filterFactoryMock;
    [SetUp]
    public void Setup()
    {
        serviceProviderMock = new();
        managedStreamMock = new();
        entityInterceptorFactoryMock = new();
        filterFactoryMock = new();
        internalMemoryTagRepository = new(entityInterceptorFactoryMock.Object, filterFactoryMock.Object);
        sut = new(serviceProviderMock.Object,
            managedStreamMock.Object, 
            internalMemoryTagRepository);
    }

    [Test]
    public void Test1()
    {

        Assert.Pass();
    }
}
