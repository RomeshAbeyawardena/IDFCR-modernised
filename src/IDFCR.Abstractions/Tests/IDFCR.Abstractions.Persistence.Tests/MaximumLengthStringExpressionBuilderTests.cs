using IDFCR.Abstractions.Persistence;
using NUnit.Framework;

namespace IDFCR.Abstractions.Persistence.Tests;

sealed class PersonModel
{
    public string? Name   { get; set; }
    public string? Email  { get; set; }
    public int     Age    { get; set; }
    public bool    Active { get; set; }
}

sealed class NoStringModel
{
    public int     Id    { get; set; }
    public decimal Value { get; set; }
    public bool    Flag  { get; set; }
}

sealed class SingleStringModel
{
    public string? Title { get; set; }
}

[TestFixture]
internal class MaximumLengthStringExpressionBuilderTests
{
    private Func<PersonModel, Dictionary<string, int>> _compiled = null!;

    [SetUp]
    public void SetUp()
    {
        _compiled = MaximumLengthStringExpressionBuilder<PersonModel>
            .BuildExpression()
            .Compile();
    }

    [Test]
    public void BuildExpression_WithPopulatedStringProperties_ReturnsRuntimeLengths()
    {
        var instance = new PersonModel { Name = "Alice", Email = "alice@example.com" };
        var result   = _compiled(instance);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result["Name"],  Is.EqualTo("Alice".Length));
            Assert.That(result["Email"], Is.EqualTo("alice@example.com".Length));
        }
    }

    [Test]
    public void BuildExpression_WithNullStringProperty_ReturnsZeroForNullProperty()
    {
        var instance = new PersonModel { Name = null, Email = "test@test.com" };
        var result   = _compiled(instance);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result["Name"],  Is.EqualTo(0));
            Assert.That(result["Email"], Is.EqualTo("test@test.com".Length));
        }
    }

    [Test]
    public void BuildExpression_WithAllNullStringProperties_ReturnsZeroForAllKeys()
    {
        var result = _compiled(new PersonModel());

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result["Name"],  Is.EqualTo(0));
            Assert.That(result["Email"], Is.EqualTo(0));
        }
    }

    [Test]
    public void BuildExpression_WithMixedPropertyTypes_OnlyIncludesStringProperties()
    {
        var result = _compiled(new PersonModel { Name = "Bob", Email = "bob@test.com", Age = 25, Active = true });

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result,                       Has.Count.EqualTo(2));
            Assert.That(result.ContainsKey("Age"),    Is.False);
            Assert.That(result.ContainsKey("Active"), Is.False);
        }
    }

    [Test]
    public void BuildExpression_WithNoStringProperties_ReturnsEmptyDictionary()
    {
        var result = MaximumLengthStringExpressionBuilder<NoStringModel>
            .BuildExpression()
            .Compile()(new NoStringModel { Id = 1, Value = 9.99m, Flag = true });

        Assert.That(result, Is.Empty);
    }

    [Test]
    public void BuildExpression_WithSingleStringProperty_ReturnsCorrectLength()
    {
        var result = MaximumLengthStringExpressionBuilder<SingleStringModel>
            .BuildExpression()
            .Compile()(new SingleStringModel { Title = "Hello, World!" });

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result,          Has.Count.EqualTo(1));
            Assert.That(result["Title"], Is.EqualTo("Hello, World!".Length));
        }
    }

    [Test]
    public void BuildExpression_AcrossMultipleInstances_AggregatesMaxLengthPerColumn()
    {
        PersonModel[] rows =
        [
            new() { Name = "Al",        Email = "al@example.com" },
            new() { Name = "Bob",        Email = "b@ex.io" },
            new() { Name = "Charlotte",  Email = "charlotte@longdomain.example.com" },
        ];

        var maxLengths = rows
            .Select(_compiled)
            .Aggregate((a, b) => a.Keys.ToDictionary(k => k, k => Math.Max(a[k], b[k])));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(maxLengths["Name"],  Is.EqualTo("Charlotte".Length));
            Assert.That(maxLengths["Email"], Is.EqualTo("charlotte@longdomain.example.com".Length));
        }
    }
}
