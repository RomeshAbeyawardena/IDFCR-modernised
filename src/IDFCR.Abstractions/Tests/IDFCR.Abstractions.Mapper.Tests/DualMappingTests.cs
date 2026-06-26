using NUnit.Framework;

namespace IDFCR.Abstractions.Mapper.Tests;

interface ISource1 : IMapper<ISource1>
{
    string Key { get; set; }
}

internal class Source1 : MapperBase<ISource1>, ISource1
{
    public string Key { get; set; } = null!;
    protected override void MapMembers(ISource1 source)
    {
        Key = source.Key;
    }
}

internal class Source2 : MapperBase<ISource2>, ISource2
{
    public string? Value { get; set; }
    protected override void MapMembers(ISource2 secondSource)
    {
        Value = secondSource.Value;
    }
}

interface ISource2 : IMapper<ISource2>
{
    string? Value { get; set; }
}

internal interface IDualMappingTestSource : IMapper<ISource1, ISource2>
{
    string Key { get; set; }
    string? Value { get; set; }
}

internal class DualMappingTestSource : MapperBase<ISource1, ISource2>, IDualMappingTestSource
{
    public string Key { get; set; } = null!;
    public string? Value { get; set; }

    protected override void MapMembers(ISource1 source)
    {
        Key = source.Key;
    }

    public override void Map(ISource2 secondSource)
    {
        Value = secondSource.Value;
    }
}

[TestFixture]
internal class DualMappingTests
{
    [Test]
    public void Test()
    {
        DualMappingTestSource testSource = new();
        Source1 source1 = new() { Key = "TestKey" };
        Source2 source2 = new() { Value = "TestValue" };

        testSource.Map(source1, source2);

        Assert.AreEqual("TestKey", testSource.Key);
        Assert.AreEqual("TestValue", testSource.Value);
    }
}
