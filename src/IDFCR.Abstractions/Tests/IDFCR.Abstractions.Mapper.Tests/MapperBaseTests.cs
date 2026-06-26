using NUnit.Framework;

namespace IDFCR.Abstractions.Mapper.Tests;

public interface IShared : IMapper<IShared>
{
    int PropertyA { get; }
    string PropertyB { get; }
    decimal PropertyC { get; }
    int? PropertyD { get; }
    bool HasProperty { get; }
}

public abstract class SharedMapperBase : MapperBase<IShared>, IShared
{
    public int PropertyA { get; set; }
    public string PropertyB { get; set; } = string.Empty;
    public decimal PropertyC { get; set; }
    public int? PropertyD { get; set; }
    public bool HasProperty { get; set; }

    protected override void MapMembers(IShared source)
    {
        PropertyA = source.PropertyA;
        PropertyB = source.PropertyB;
        PropertyC = source.PropertyC;
        PropertyD = source.PropertyD;
        HasProperty = source.HasProperty;
    }
}

public sealed class Shared : SharedMapperBase;

public sealed class SharedDto : SharedMapperBase;

public sealed class SharedEntity : SharedMapperBase;

public sealed class SharedWithMetadataDto(string tag, int version) : SharedMapperBase
{
    public string Tag { get; } = tag;
    public int Version { get; } = version;
}

[TestFixture]
public class MapperBaseTests
{
    private Shared shared = null!;

    [SetUp]
    public void SetUp()
    {
        shared = new Shared
        {
            PropertyA = 42,
            PropertyB = "Hello",
            PropertyC = 3.14m,
            PropertyD = null,
            HasProperty = true
        };
    }

    [Test]
    public void Map_WithoutParameters_MapsAllProperties_ToNewInstance()
    {
        var sharedDto = shared.Map<SharedDto>();

        Assert.That(sharedDto, Is.Not.Null);
        Assert.That(sharedDto, Is.Not.TypeOf<Shared>());
        AssertSharedValues(shared, sharedDto);
    }

    [Test]
    public void Map_WithParameters_UsesConstructorArguments_AndMapsValues()
    {
        var mapped = shared.Map<SharedWithMetadataDto>("dto-v1", 7);

        Assert.That(mapped, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(mapped!.Tag, Is.EqualTo("dto-v1"));
            Assert.That(mapped.Version, Is.EqualTo(7));
        }

        AssertSharedValues(shared, mapped);
    }

    [Test]
    public void Map_WithInvalidConstructorParameters_ThrowsMissingMethodException()
    {
        Assert.Throws<MissingMethodException>(() => shared.Map<SharedWithMetadataDto>(123, "wrong-order"));
    }

    [Test]
    public void Map_CanChainAcrossTypes_PreservingValues()
    {
        var sharedDto = shared.Map<SharedDto>();
        var sharedEntity = sharedDto.Map<SharedEntity>();

        AssertSharedValues(shared, sharedDto);
        AssertSharedValues(shared, sharedEntity);
    }

    private static void AssertSharedValues(IShared expected, IShared actual)
    {
        using (Assert.EnterMultipleScope())
        {
            Assert.That(actual.PropertyA, Is.EqualTo(expected.PropertyA));
            Assert.That(actual.PropertyB, Is.EqualTo(expected.PropertyB));
            Assert.That(actual.PropertyC, Is.EqualTo(expected.PropertyC));
            Assert.That(actual.PropertyD, Is.EqualTo(expected.PropertyD));
            Assert.That(actual.HasProperty, Is.EqualTo(expected.HasProperty));
        }
    }
}
