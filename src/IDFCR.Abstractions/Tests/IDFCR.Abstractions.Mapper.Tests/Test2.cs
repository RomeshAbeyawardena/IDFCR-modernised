using IDFCR.Abstractions.Mapper.Extensions;
using NUnit.Framework;

namespace IDFCR.Abstractions.Mapper.Tests;

public interface IDtoWithAutoMapping : IMapper<IDtoWithAutoMapping>
{
    string Name { get; }
    int A { get; }
    decimal B { get; }
    float C { get; }
    bool IsValue { get; }
}

public class DtoWithAutoMapping : MapperBase<IDtoWithAutoMapping>, IDtoWithAutoMapping
{
    public string Name { get; set; } = null!;
    public int A { get; set; }
    public decimal B { get; set; }
    public float C { get; set; }
    public bool IsValue { get; set; }

    public override void Map(IDtoWithAutoMapping source)
    {
        this.SingularMap(source);
    }
}

internal class DtoWithAutoMappingManualClone : MapperBase<IDtoWithAutoMapping>, IDtoWithAutoMapping
{
    public string Name { get; set; } = null!;
    public int A { get; set; }
    public decimal B { get; set; }
    public float C { get; set; }
    public bool IsValue { get; set; }

    public override void Map(IDtoWithAutoMapping source)
    {
        Name = source.Name;
        A = source.A;
        B = source.B;
        C = source.C;
        IsValue = source.IsValue;
    }
}

[TestFixture]
internal class AutoTests
{
    private static DtoWithAutoMappingManualClone CreateSource() => new()
    {
        Name = "Test",
        A = 4,
        B = 2.2m,
        C = 2.4293F,
        IsValue = true
    };

    /// <summary>
    /// Proves that SingularMap produces the same result as a manual one-to-one clone.
    /// </summary>
    [Test]
    public void SingularMap_ProducesSameResultAsManualClone()
    {
        var source = CreateSource();

        var autoMapped = source.Map<DtoWithAutoMapping>();
        var manualMapped = source.Map<DtoWithAutoMappingManualClone>();

        Assert.Multiple(() =>
        {
            Assert.That(autoMapped.Name,    Is.EqualTo(manualMapped.Name),    "Name mismatch");
            Assert.That(autoMapped.A,       Is.EqualTo(manualMapped.A),       "A mismatch");
            Assert.That(autoMapped.B,       Is.EqualTo(manualMapped.B),       "B mismatch");
            Assert.That(autoMapped.C,       Is.EqualTo(manualMapped.C),       "C mismatch");
            Assert.That(autoMapped.IsValue, Is.EqualTo(manualMapped.IsValue), "IsValue mismatch");
        });
    }

    /// <summary>
    /// Proves that SingularMap copies each property individually to expected values.
    /// </summary>
    [Test]
    public void SingularMap_CopiesAllProperties_ToExpectedValues()
    {
        var source = CreateSource();

        var autoMapped = source.Map<DtoWithAutoMapping>();

        Assert.Multiple(() =>
        {
            Assert.That(autoMapped.Name,    Is.EqualTo("Test"),     "Name mismatch");
            Assert.That(autoMapped.A,       Is.EqualTo(4),          "A mismatch");
            Assert.That(autoMapped.B,       Is.EqualTo(2.2m),       "B mismatch");
            Assert.That(autoMapped.C,       Is.EqualTo(2.4293F),    "C mismatch");
            Assert.That(autoMapped.IsValue, Is.True,                "IsValue mismatch");
        });
    }

    /// <summary>
    /// Proves that SingularMap does not throw when source is null (guard clause).
    /// </summary>
    [Test]
    public void SingularMap_DoesNotThrow_WhenSourceIsNull()
    {
        var target = new DtoWithAutoMapping();

        Assert.DoesNotThrow(() => target.SingularMap<IDtoWithAutoMapping, DtoWithAutoMapping>(null!));
    }
}
