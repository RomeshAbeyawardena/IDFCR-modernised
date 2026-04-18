using BuildTools.Cli.Features.Tags;
using BuildTools.Infrastructure.Features.Tags;
using BuildTools.Infrastructure.SqlServer.Features.Tags;
using BuildTools.Shared.Contracts.Features.Tags;
using BuildTools.Shared.Features.Tags;
using IDFCR.Abstractions.Cli.ManagedStreams;
using IDFCR.Abstractions.Filters;
using IDFCR.Abstractions.Interceptors;
using IDFCR.Abstractions.Results;
using IDFCR.TestUtilities;
using MediatR;
using Moq;
using ContractTagDto = BuildTools.Shared.Contracts.Features.Tags.TagDto;

namespace BuildTools.Cli.Tests;

public class InternalMemoryTagRepository(IEntityInterceptorFactory entityInterceptorFactory, IFilterFactory filterFactory)
    : InternalMemoryMockRepository<ITag, TagEntity, Tag>(entityInterceptorFactory, filterFactory), ITagRepository
{
    internal List<TagEntity> Entities => base.Entries;
    public async Task<IUnitResult> AddTagsAsync(IEnumerable<Tag> tags, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
        base.Entries.AddRange(tags.Select(x =>
        {
            var entity = x.Map<TagEntity>();
            if (entity.Id == Guid.Empty)
            {
                entity.Id = Guid.NewGuid();
            }

            return entity;
        }));
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
    private TagWriteOperation sut = null!;

    private Mock<IServiceProvider> serviceProviderMock = null!;
    private Mock<IManagedStream> managedStreamMock = null!;
    private Mock<IMediator> mediatorMock = null!;
    private InternalMemoryTagRepository internalMemoryTagRepository = null!;
    private Mock<IEntityInterceptorFactory> entityInterceptorFactoryMock = null!;
    private Mock<IFilterFactory> filterFactoryMock = null!;

    [SetUp]
    public void Setup()
    {
        serviceProviderMock = new();
        managedStreamMock = new();
        mediatorMock = new();
        entityInterceptorFactoryMock = new();
        filterFactoryMock = new();
        internalMemoryTagRepository = new(entityInterceptorFactoryMock.Object, filterFactoryMock.Object);

        mediatorMock
            .Setup(x => x.Send(It.IsAny<GetTagsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((GetTagsQuery query, CancellationToken _) =>
                UnitResultCollection.FromResult(
                    internalMemoryTagRepository.Entities
                        .Where(x => query.Names.Contains(x.Name, StringComparer.OrdinalIgnoreCase))
                        .Select(x => new ContractTagDto
                        {
                            Id = x.Id,
                            Name = x.Name,
                            DisplayName = x.DisplayName
                        })));

        mediatorMock
            .Setup(x => x.Send(It.IsAny<GetTagQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((GetTagQuery query, CancellationToken _) =>
            {
                var entry = internalMemoryTagRepository.Entities
                    .FirstOrDefault(x => query.Name is not null && x.Name.Equals(query.Name, StringComparison.OrdinalIgnoreCase));

                return entry is null
                    ? UnitResult.NotFound<ContractTagDto>(query.Name ?? string.Empty)
                    : UnitResult.FromResult(new ContractTagDto
                    {
                        Id = entry.Id,
                        Name = entry.Name,
                        DisplayName = entry.DisplayName
                    });
            });

        sut = new(serviceProviderMock.Object, managedStreamMock.Object, mediatorMock.Object, internalMemoryTagRepository);
    }

    [Test]
    public async Task MultipleUpsert_ShouldBeIdempotent_WhenCalledWithSameTags()
    {
        // Arrange
        const string tagsToInsert = "tag1:Tag 1,tag2:Tag 2,tag3:Tag 3,tag4:Tag 4";

        // Act
        await sut.MultipleUpsert(tagsToInsert, CancellationToken.None);
        await sut.MultipleUpsert(tagsToInsert, CancellationToken.None);

        // Assert
        Assert.That(internalMemoryTagRepository.Entities, Has.Count.EqualTo(4));
        Assert.That(
            internalMemoryTagRepository.Entities.Select(x => x.Name),
            Is.EquivalentTo(["tag1", "tag2", "tag3", "tag4"]));
    }

    [Test]
    public async Task MultipleUpsert_ShouldNotInsertDuplicateTags_WhenNamesDifferOnlyByCase()
    {
        // Arrange
        const string initialTags = "tag1:Tag 1,tag2:Tag 2";
        const string secondImport = "TAG1:Updated Name,tag3:Tag 3";

        // Act
        await sut.MultipleUpsert(initialTags, CancellationToken.None);
        await sut.MultipleUpsert(secondImport, CancellationToken.None);

        // Assert
        Assert.That(internalMemoryTagRepository.Entities, Has.Count.EqualTo(3));
        Assert.That(
            internalMemoryTagRepository.Entities.Select(x => x.Name),
            Is.EquivalentTo(["tag1", "tag2", "tag3"]));
    }

    [Test]
    public async Task MultipleUpsert_ShouldIgnoreMalformedEntries()
    {
        // Arrange
        const string tagsWithInvalidItems = "tag1:Tag 1,invalid-entry,tag2:Tag 2:extra,tag3:Tag 3";

        // Act
        await sut.MultipleUpsert(tagsWithInvalidItems, CancellationToken.None);

        // Assert
        Assert.That(internalMemoryTagRepository.Entities, Has.Count.EqualTo(2));
        Assert.That(
            internalMemoryTagRepository.Entities.Select(x => x.Name),
            Is.EquivalentTo(["tag1", "tag3"]));
    }

    [Test]
    public async Task MultipleUpsert_ShouldTrimWhitespaceAroundDelimitedValues()
    {
        // Arrange
        const string tagsToInsert = "  tag1 :  Tag 1  ,   tag2:   Tag 2   ";

        // Act
        await sut.MultipleUpsert(tagsToInsert, CancellationToken.None);

        // Assert
        Assert.That(internalMemoryTagRepository.Entities, Has.Count.EqualTo(2));
        Assert.That(
            internalMemoryTagRepository.Entities.Select(x => x.Name),
            Is.EquivalentTo(["tag1", "tag2"]));
        Assert.That(
            internalMemoryTagRepository.Entities.Select(x => x.DisplayName),
            Is.EquivalentTo(["Tag 1", "Tag 2"]));
    }

    [Test]
    public async Task MultipleUpsert_ShouldIgnoreEmptyCsvSegments()
    {
        // Arrange
        const string tagsToInsert = "tag1:Tag 1,, ,tag2:Tag 2,";

        // Act
        await sut.MultipleUpsert(tagsToInsert, CancellationToken.None);

        // Assert
        Assert.That(internalMemoryTagRepository.Entities, Has.Count.EqualTo(2));
        Assert.That(
            internalMemoryTagRepository.Entities.Select(x => x.Name),
            Is.EquivalentTo(["tag1", "tag2"]));
    }
}
