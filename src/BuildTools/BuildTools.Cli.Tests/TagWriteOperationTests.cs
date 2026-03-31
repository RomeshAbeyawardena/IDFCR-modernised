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
using System.Linq;

namespace BuildTools.Cli.Tests;

public class InternalMemoryTagRepository(IEntityInterceptorFactory entityInterceptorFactory, IFilterFactory filterFactory)
    : InternalMemoryMockRepository<ITag, TagEntity, Tag>(entityInterceptorFactory, filterFactory), ITagRepository
{
    public async Task<IUnitResult> AddTagsAsync(IEnumerable<Tag> tags, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
        base.Entries.AddRange(tags.Select(x => x.Map<TagEntity>()));
        return UnitResult.Success(UnitAction.Add);
    }

    public async Task<IUnitResultCollection<Tag>> GetExistingTagsAsync(IEnumerable<string> tags, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
        var foundTags = Entries.Where(x => tags.Contains(x.Name, StringComparer.OrdinalIgnoreCase))
            .Select(x => x.Map<Tag>());

        return UnitResultCollection.FromResult(foundTags);
    }

    public async Task<IUnitResult<Tag>> GetTagAsync(string name, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
        var entry = Entries.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

        if (entry is null)
        {
            return UnitResult.NotFound<Tag>(name);
        }

        return UnitResult.FromResult(entry.Map<Tag>());
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
    public async Task Test1()
    {
        string tagsToInsert = "";
        await sut.MultipleUpsert(tagsToInsert, CancellationToken.None);
    }
}
