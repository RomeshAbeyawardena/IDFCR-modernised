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

/// <summary>
/// Opts into automatic mapping by inheriting <see cref="AutoMapperBase{TSource}"/>.
/// The protected SingularMap is only reachable through this deliberate inheritance — 
/// external callers cannot invoke it directly.
/// </summary>
public class DtoWithAutoMapping : AutoMapperBase<IDtoWithAutoMapping>, IDtoWithAutoMapping
{
    public string Name { get; set; } = null!;
    public int A { get; set; }
    public decimal B { get; set; }
    public float C { get; set; }
    public bool IsValue { get; set; }

    protected override void MapMembers(IDtoWithAutoMapping source)
    {
        SingularMap(source);
    }
}

/// <summary>
/// Intentionally does NOT inherit <see cref="AutoMapperBase{TSource}"/> — 
/// proves manual mapping remains the baseline and that the auto path is an explicit opt-in.
/// </summary>
internal class DtoWithAutoMappingManualClone : MapperBase<IDtoWithAutoMapping>, IDtoWithAutoMapping
{
    public string Name { get; set; } = null!;
    public int A { get; set; }
    public decimal B { get; set; }
    public float C { get; set; }
    public bool IsValue { get; set; }

    protected override void MapMembers(IDtoWithAutoMapping source)
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
    /// Proves AutoMapperBase.SingularMap produces an identical result to a hand-written clone,
    /// validating the opt-in auto-mapping path against the manual baseline.
    /// </summary>
    [Test]
    public void SingularMap_ProducesSameResultAsManualClone()
    {
        var source = CreateSource();

        var autoMapped   = source.Map<DtoWithAutoMapping>();
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
    /// Proves that every property is copied to its exact expected value via AutoMapperBase.SingularMap.
    /// </summary>
    [Test]
    public void SingularMap_CopiesAllProperties_ToExpectedValues()
    {
        var source = CreateSource();

        var autoMapped = source.Map<DtoWithAutoMapping>();

        Assert.Multiple(() =>
        {
            Assert.That(autoMapped.Name,    Is.EqualTo("Test"),  "Name mismatch");
            Assert.That(autoMapped.A,       Is.EqualTo(4),       "A mismatch");
            Assert.That(autoMapped.B,       Is.EqualTo(2.2m),    "B mismatch");
            Assert.That(autoMapped.C,       Is.EqualTo(2.4293F), "C mismatch");
            Assert.That(autoMapped.IsValue, Is.True,             "IsValue mismatch");
        });
    }

    /// <summary>
    /// Proves the boundary holds: a type that does not inherit AutoMapperBase
    /// has no access to SingularMap — the opt-in gate is enforced at the type level.
    /// This is a compile-time guarantee; the test documents the architectural intent.
    /// </summary>
    [Test]
    public void ManualClone_DoesNotInheritAutoMapperBase_EnforcingOptInBoundary()
    {
        Assert.That(
            typeof(DtoWithAutoMappingManualClone).IsSubclassOf(typeof(AutoMapperBase<IDtoWithAutoMapping>)),
            Is.False,
            "DtoWithAutoMappingManualClone must NOT inherit AutoMapperBase — " +
            "SingularMap access should remain an explicit opt-in via inheritance only.");
    }
}
