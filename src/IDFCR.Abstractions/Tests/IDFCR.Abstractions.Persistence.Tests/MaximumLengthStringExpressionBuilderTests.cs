using NUnit.Framework;

namespace IDFCR.Abstractions.Persistence.Tests;

sealed class PersonModel
{
    public string? Name { get; set; }
    public string? Email { get; set; }
    public int Age { get; set; }
    public bool Active { get; set; }
}

sealed class NoStringModel
{
    public int Id { get; set; }
    public decimal Value { get; set; }
    public bool Flag { get; set; }
}

sealed class SingleStringModel
{
    public string? Title { get; set; }
}

[TestFixture]
internal class MaximumLengthStringExpressionBuilderTests
{
    private Func<PersonModel, IEnumerable<KeyValuePair<string, int>>> _compiled = null!;

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
        var result = _compiled(instance);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(ValueOf(result, "Name"), Is.EqualTo("Alice".Length));
            Assert.That(ValueOf(result, "Email"), Is.EqualTo("alice@example.com".Length));
        }
    }

    [Test]
    public void BuildExpression_WithNullStringProperty_ReturnsZeroForNullProperty()
    {
        var result = _compiled(new PersonModel { Name = null, Email = "test@test.com" });

        using (Assert.EnterMultipleScope())
        {
            Assert.That(ValueOf(result, "Name"), Is.EqualTo(0));
            Assert.That(ValueOf(result, "Email"), Is.EqualTo("test@test.com".Length));
        }
    }

    [Test]
    public void BuildExpression_WithAllNullStringProperties_ReturnsZeroForAllKeys()
    {
        var result = _compiled(new PersonModel());

        using (Assert.EnterMultipleScope())
        {
            Assert.That(ValueOf(result, "Name"), Is.EqualTo(0));
            Assert.That(ValueOf(result, "Email"), Is.EqualTo(0));
        }
    }

    [Test]
    public void BuildExpression_WithMixedPropertyTypes_OnlyIncludesStringProperties()
    {
        var result = _compiled(new PersonModel { Name = "Bob", Email = "bob@test.com", Age = 25, Active = true });

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Has.Exactly(2).Items);
            Assert.That(result.Any(kvp => kvp.Key == "Age"), Is.False);
            Assert.That(result.Any(kvp => kvp.Key == "Active"), Is.False);
        }
    }

    [Test]
    public void BuildExpression_WithNoStringProperties_ReturnsEmptySequence()
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
            Assert.That(result, Has.Exactly(1).Items);
            Assert.That(ValueOf(result, "Title"), Is.EqualTo("Hello, World!".Length));
        }
    }

    [Test]
    public void BuildExpression_AcrossMultipleInstances_AggregatesMaxLengthPerColumn()
    {
        PersonModel[] rows =
        [
            new() { Name = "Al",       Email = "al@example.com"                    },
            new() { Name = "Bob",      Email = "b@ex.io"                           },
            new() { Name = "Charlotte", Email = "charlotte@longdomain.example.com" },
        ];

        var maxLengths = rows
            .SelectMany(_compiled)
            .GroupBy(kvp => kvp.Key)
            .ToDictionary(g => g.Key, g => g.Max(kvp => kvp.Value));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(maxLengths["Name"], Is.EqualTo("Charlotte".Length));
            Assert.That(maxLengths["Email"], Is.EqualTo("charlotte@longdomain.example.com".Length));
        }
    }

    private static int ValueOf(IEnumerable<KeyValuePair<string, int>> pairs, string key)
        => pairs.Single(kvp => kvp.Key == key).Value;
}
