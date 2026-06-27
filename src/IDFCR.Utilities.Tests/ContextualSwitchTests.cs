using NUnit.Framework;

namespace IDFCR.Utilities.Tests;

[TestFixture]
internal class ContextualSwitchTests
{
    #region Test Context Types

    private record EvaluationContext(string UserId, DateTime Timestamp, int Priority);

    #endregion

    #region Basic Contextual Switch Creation Tests

    [Test]
    public void BuildContextual_WithSimpleValueCases_ReturnsCorrectValues()
    {
        // Arrange & Act
        var sut = SwitchBuilder.BuildContextual<int, string, string>(config =>
        {
            config.CaseWhen(1, "one");
            config.CaseWhen(2, "two");
            config.CaseWhen(3, "three");
        });

        var context = "ignored";

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(sut.ThenValue(1, context), Is.EqualTo("one"));
            Assert.That(sut.ThenValue(2, context), Is.EqualTo("two"));
            Assert.That(sut.ThenValue(3, context), Is.EqualTo("three"));
        });
    }

    [Test]
    public void BuildContextual_WithValueFactoryCases_ReturnsCorrectValues()
    {
        // Arrange & Act
        var sut = SwitchBuilder.BuildContextual<int, string, string>(config =>
        {
            config.CaseWhen(1, (key, ctx) => $"Key: {key}, Context: {ctx}");
            config.CaseWhen(2, (key, ctx) => $"Key: {key}, Context: {ctx}");
            config.CaseWhen(3, (key, ctx) => $"Key: {key}, Context: {ctx}");
        });

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(sut.ThenValue(1, "alpha"), Is.EqualTo("Key: 1, Context: alpha"));
            Assert.That(sut.ThenValue(2, "beta"), Is.EqualTo("Key: 2, Context: beta"));
            Assert.That(sut.ThenValue(3, "gamma"), Is.EqualTo("Key: 3, Context: gamma"));
        });
    }

    [Test]
    public void BuildContextual_WithNoMatchingCase_ReturnsDefault()
    {
        // Arrange
        var sut = SwitchBuilder.BuildContextual<int, string, string>(config =>
        {
            config.CaseWhen(1, "one");
            config.CaseWhen(2, "two");
        });

        // Act
        var result = sut.ThenValue(999, "context");

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public void BuildContextual_WithEmptyConfiguration_ReturnsDefaultTValueForAnyKey()
    {
        // Arrange
        var sut = SwitchBuilder.BuildContextual<string, int, int>(config => { });

        // Act & Assert
        // Returns default(TValue) which is 0 for int
        Assert.Multiple(() =>
        {
            Assert.That(sut.ThenValue("any", 42), Is.EqualTo(default(int)));
            Assert.That(sut.ThenValue("key", 99), Is.EqualTo(default(int)));
        });
    }

    #endregion

    #region Context Usage Tests

    [Test]
    public void BuildContextual_WithTupleContext_UsesContextCorrectly()
    {
        // Arrange
        var sut = SwitchBuilder.BuildContextual<string, (int UserId, string Action), string>(config =>
        {
            config.CaseWhen("admin", (key, ctx) => $"Admin {ctx.UserId} performed {ctx.Action}");
            config.CaseWhen("user", (key, ctx) => $"User {ctx.UserId} performed {ctx.Action}");
        });

        // Act
        var result1 = sut.ThenValue("admin", (100, "login"));
        var result2 = sut.ThenValue("user", (200, "logout"));

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result1, Is.EqualTo("Admin 100 performed login"));
            Assert.That(result2, Is.EqualTo("User 200 performed logout"));
        });
    }

    [Test]
    public void BuildContextual_WithRecordContext_UsesContextCorrectly()
    {
        // Arrange
        var sut = SwitchBuilder.BuildContextual<string, EvaluationContext, string>(config =>
        {
            config.CaseWhen("process", (key, ctx) => 
                $"Processing for {ctx.UserId} at {ctx.Timestamp:yyyy-MM-dd} with priority {ctx.Priority}");
            config.CaseWhen("approve", (key, ctx) => 
                $"Approved by {ctx.UserId} with priority {ctx.Priority}");
        });

        var context = new EvaluationContext("admin@test.com", new DateTime(2024, 1, 15), 5);

        // Act
        var result = sut.ThenValue("process", context);

        // Assert
        Assert.That(result, Is.EqualTo("Processing for admin@test.com at 2024-01-15 with priority 5"));
    }

    [Test]
    public void BuildContextual_DifferentContextValues_ProduceDifferentResults()
    {
        // Arrange
        var sut = SwitchBuilder.BuildContextual<int, int, int>(config =>
        {
            config.CaseWhen(1, (key, multiplier) => key * multiplier);
            config.CaseWhen(2, (key, multiplier) => key * multiplier);
        });

        // Act & Assert
        Assert.Multiple(() =>
        {
            Assert.That(sut.ThenValue(1, 10), Is.EqualTo(10));
            Assert.That(sut.ThenValue(1, 20), Is.EqualTo(20));
            Assert.That(sut.ThenValue(2, 10), Is.EqualTo(20));
            Assert.That(sut.ThenValue(2, 5), Is.EqualTo(10));
        });
    }

    #endregion

    #region Default Case (Else) Tests

    [Test]
    public void BuildContextual_WithElseValue_ReturnsElseValueForNonMatchingKey()
    {
        // Arrange
        var sut = SwitchBuilder.BuildContextual<int, string, string>(config =>
        {
            config.CaseWhen(1, "one");
            config.CaseWhen(2, "two");
            config.Else("default");
        });

        // Act & Assert
        Assert.Multiple(() =>
        {
            Assert.That(sut.ThenValue(1, "ctx"), Is.EqualTo("one"));
            Assert.That(sut.ThenValue(999, "ctx"), Is.EqualTo("default"));
            Assert.That(sut.ThenValue(-1, "ctx"), Is.EqualTo("default"));
        });
    }

    [Test]
    public void BuildContextual_WithElseValueFactory_ReturnsElseValueForNonMatchingKey()
    {
        // Arrange
        var sut = SwitchBuilder.BuildContextual<int, string, string>(config =>
        {
            config.CaseWhen(1, "one");
            config.CaseWhen(2, "two");
            config.Else((key, ctx) => $"Unknown key: {key} with context: {ctx}");
        });

        // Act & Assert
        Assert.Multiple(() =>
        {
            Assert.That(sut.ThenValue(1, "alpha"), Is.EqualTo("one"));
            Assert.That(sut.ThenValue(999, "beta"), Is.EqualTo("Unknown key: 999 with context: beta"));
            Assert.That(sut.ThenValue(-1, "gamma"), Is.EqualTo("Unknown key: -1 with context: gamma"));
        });
    }

    [Test]
    public void BuildContextual_ElseCalledMultipleTimes_UsesLastElseValue()
    {
        // Arrange
        var sut = SwitchBuilder.BuildContextual<int, string, string>(config =>
        {
            config.CaseWhen(1, "one");
            config.Else("first default");
            config.Else("second default");
            config.Else("third default");
        });

        // Act
        var result = sut.ThenValue(999, "context");

        // Assert
        Assert.That(result, Is.EqualTo("third default"));
    }

    #endregion

    #region Case Replacement Tests

    [Test]
    public void BuildContextual_CaseDefinedMultipleTimes_UsesLastDefinedValue()
    {
        // Arrange
        var sut = SwitchBuilder.BuildContextual<int, string, string>(config =>
        {
            config.CaseWhen(1, "first");
            config.CaseWhen(1, "second");
            config.CaseWhen(1, "third");
        });

        // Act
        var result = sut.ThenValue(1, "context");

        // Assert
        Assert.That(result, Is.EqualTo("third"));
    }

    [Test]
    public void BuildContextual_CaseReplacedWithValueFactory_UsesLatestFactory()
    {
        // Arrange
        var counter = 0;
        var sut = SwitchBuilder.BuildContextual<int, string, string>(config =>
        {
            config.CaseWhen(1, "static value");
            config.CaseWhen(1, (key, ctx) => $"{ctx}-{++counter}");
        });

        // Act
        var result1 = sut.ThenValue(1, "test");
        var result2 = sut.ThenValue(1, "test");

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result1, Is.EqualTo("test-1"));
            Assert.That(result2, Is.EqualTo("test-2"));
        });
    }

    #endregion

    #region Then Method Tests

    [Test]
    public void Then_WithMatchingKey_ReturnsValueFactory()
    {
        // Arrange
        var sut = SwitchBuilder.BuildContextual<int, string, string>(config =>
        {
            config.CaseWhen(1, (key, ctx) => $"Key: {key}, Context: {ctx}");
        });

        // Act
        var factory = sut.Then(1);

        // Assert
        Assert.That(factory, Is.Not.Null);
        Assert.That(factory!(1, "alpha"), Is.EqualTo("Key: 1, Context: alpha"));
    }

    [Test]
    public void Then_WithNonMatchingKeyAndNoElse_ReturnsNull()
    {
        // Arrange
        var sut = SwitchBuilder.BuildContextual<int, string, string>(config =>
        {
            config.CaseWhen(1, "one");
        });

        // Act
        var factory = sut.Then(999);

        // Assert
        Assert.That(factory, Is.Null);
    }

    [Test]
    public void Then_WithNonMatchingKeyAndElse_ReturnsElseFactory()
    {
        // Arrange
        var sut = SwitchBuilder.BuildContextual<int, string, string>(config =>
        {
            config.CaseWhen(1, "one");
            config.Else((key, ctx) => $"Default: {key}-{ctx}");
        });

        // Act
        var factory = sut.Then(999);

        // Assert
        Assert.That(factory, Is.Not.Null);
        Assert.That(factory!(999, "beta"), Is.EqualTo("Default: 999-beta"));
    }

    #endregion

    #region Source-Based Build Tests

    [Test]
    public void BuildContextual_WithSourceSwitch_PreservesExistingCases()
    {
        // Arrange
        var source = SwitchBuilder.BuildContextual<int, string, string>(config =>
        {
            config.CaseWhen(1, (k, c) => $"{k}-{c}");
            config.CaseWhen(2, "two");
            config.Else("default");
        });

        // Act
        var extended = SwitchBuilder.BuildContextual(source, config =>
        {
            config.CaseWhen(3, "three");
        });

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(extended.ThenValue(1, "alpha"), Is.EqualTo("1-alpha"));
            Assert.That(extended.ThenValue(2, "beta"), Is.EqualTo("two"));
            Assert.That(extended.ThenValue(3, "gamma"), Is.EqualTo("three"));
            Assert.That(extended.ThenValue(999, "delta"), Is.EqualTo("default"));
        });
    }

    [Test]
    public void BuildContextual_WithSourceSwitch_CanOverrideExistingCases()
    {
        // Arrange
        var source = SwitchBuilder.BuildContextual<int, string, string>(config =>
        {
            config.CaseWhen(1, "one");
            config.CaseWhen(2, "two");
        });

        // Act
        var modified = SwitchBuilder.BuildContextual(source, config =>
        {
            config.CaseWhen(1, (k, c) => $"ONE-{c}");
            config.CaseWhen(2, (k, c) => $"TWO-{c}");
        });

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(modified.ThenValue(1, "ctx"), Is.EqualTo("ONE-ctx"));
            Assert.That(modified.ThenValue(2, "ctx"), Is.EqualTo("TWO-ctx"));
        });
    }

    [Test]
    public void BuildContextual_WithSourceSwitch_CanReplaceElseClause()
    {
        // Arrange
        var source = SwitchBuilder.BuildContextual<int, string, string>(config =>
        {
            config.CaseWhen(1, "one");
            config.Else("original default");
        });

        // Act
        var modified = SwitchBuilder.BuildContextual(source, config =>
        {
            config.Else((k, c) => $"new default-{c}");
        });

        // Assert
        Assert.That(modified.ThenValue(999, "test"), Is.EqualTo("new default-test"));
    }

    [Test]
    public void BuildContextual_WithSourceSwitch_OriginalSwitchRemainsUnchanged()
    {
        // Arrange
        var source = SwitchBuilder.BuildContextual<int, string, string>(config =>
        {
            config.CaseWhen(1, "one");
            config.Else("original");
        });

        // Act
        var modified = SwitchBuilder.BuildContextual(source, config =>
        {
            config.CaseWhen(1, "ONE");
            config.CaseWhen(2, "TWO");
            config.Else("modified");
        });

        // Assert - verify source is unchanged
        Assert.Multiple(() =>
        {
            Assert.That(source.ThenValue(1, "ctx"), Is.EqualTo("one"));
            Assert.That(source.ThenValue(2, "ctx"), Is.EqualTo("original"));
            Assert.That(source.ThenValue(999, "ctx"), Is.EqualTo("original"));
        });

        // Assert - verify modified is correct
        Assert.Multiple(() =>
        {
            Assert.That(modified.ThenValue(1, "ctx"), Is.EqualTo("ONE"));
            Assert.That(modified.ThenValue(2, "ctx"), Is.EqualTo("TWO"));
            Assert.That(modified.ThenValue(999, "ctx"), Is.EqualTo("modified"));
        });
    }

    #endregion

    #region Method Chaining Tests

    [Test]
    public void CaseWhen_ReturnsBuilder_AllowsMethodChaining()
    {
        // Arrange & Act
        var sut = SwitchBuilder.BuildContextual<int, string, string>(config =>
        {
            var builder = config.CaseWhen(1, "one")
                                .CaseWhen(2, "two")
                                .CaseWhen(3, "three")
                                .Else("default");

            Assert.That(builder, Is.SameAs(config));
        });

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(sut.ThenValue(1, "ctx"), Is.EqualTo("one"));
            Assert.That(sut.ThenValue(2, "ctx"), Is.EqualTo("two"));
            Assert.That(sut.ThenValue(3, "ctx"), Is.EqualTo("three"));
        });
    }

    #endregion

    #region Different Key Type Tests

    [Test]
    public void BuildContextual_WithStringKeys_WorksCorrectly()
    {
        // Arrange
        var sut = SwitchBuilder.BuildContextual<string, int, int>(config =>
        {
            config.CaseWhen("one", (k, multiplier) => 1 * multiplier);
            config.CaseWhen("two", (k, multiplier) => 2 * multiplier);
            config.CaseWhen("three", (k, multiplier) => 3 * multiplier);
            config.Else((k, multiplier) => -1 * multiplier);
        });

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(sut.ThenValue("one", 10), Is.EqualTo(10));
            Assert.That(sut.ThenValue("two", 10), Is.EqualTo(20));
            Assert.That(sut.ThenValue("three", 10), Is.EqualTo(30));
            Assert.That(sut.ThenValue("unknown", 10), Is.EqualTo(-10));
        });
    }

    [Test]
    public void BuildContextual_WithEnumKeys_WorksCorrectly()
    {
        // Arrange
        var sut = SwitchBuilder.BuildContextual<DayOfWeek, int, string>(config =>
        {
            config.CaseWhen(DayOfWeek.Monday, (day, hrs) => $"{day}: {hrs} hours work");
            config.CaseWhen(DayOfWeek.Friday, (day, hrs) => $"{day}: {hrs} hours work");
            config.CaseWhen(DayOfWeek.Saturday, (day, hrs) => $"{day}: {hrs} hours weekend");
            config.Else((day, hrs) => $"{day}: {hrs} hours regular");
        });

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(sut.ThenValue(DayOfWeek.Monday, 8), Is.EqualTo("Monday: 8 hours work"));
            Assert.That(sut.ThenValue(DayOfWeek.Friday, 8), Is.EqualTo("Friday: 8 hours work"));
            Assert.That(sut.ThenValue(DayOfWeek.Saturday, 0), Is.EqualTo("Saturday: 0 hours weekend"));
            Assert.That(sut.ThenValue(DayOfWeek.Tuesday, 8), Is.EqualTo("Tuesday: 8 hours regular"));
        });
    }

    #endregion

    #region Complex Value Tests

    [Test]
    public void BuildContextual_WithComplexValueTypes_WorksCorrectly()
    {
        // Arrange
        var sut = SwitchBuilder.BuildContextual<int, string, List<string>>(config =>
        {
            config.CaseWhen(1, new List<string> { "one" });
            config.CaseWhen(2, (k, ctx) => new List<string> { "two", ctx });
            config.Else((k, ctx) => new List<string> { "default", ctx, k.ToString() });
        });

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(sut.ThenValue(1, "alpha"), Has.Count.EqualTo(1));
            Assert.That(sut.ThenValue(1, "alpha")![0], Is.EqualTo("one"));
            Assert.That(sut.ThenValue(2, "beta"), Has.Count.EqualTo(2));
            Assert.That(sut.ThenValue(2, "beta")![1], Is.EqualTo("beta"));
            Assert.That(sut.ThenValue(999, "gamma"), Has.Count.EqualTo(3));
            Assert.That(sut.ThenValue(999, "gamma")![1], Is.EqualTo("gamma"));
            Assert.That(sut.ThenValue(999, "gamma")![2], Is.EqualTo("999"));
        });
    }

    [Test]
    public void BuildContextual_WithNullableValueTypes_WorksCorrectly()
    {
        // Arrange
        var sut = SwitchBuilder.BuildContextual<int, int, int?>(config =>
        {
            config.CaseWhen(1, 100);
            config.CaseWhen(2, (int?)null);
            config.Else((key, multiplier) => key * multiplier);
        });

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(sut.ThenValue(1, 10), Is.EqualTo(100));
            Assert.That(sut.ThenValue(2, 10), Is.Null);
            Assert.That(sut.ThenValue(5, 10), Is.EqualTo(50));
        });
    }

    #endregion

    #region State Isolation Tests

    [Test]
    public void BuildContextual_MultipleSwitches_AreIndependent()
    {
        // Arrange & Act
        var switch1 = SwitchBuilder.BuildContextual<int, string, string>(config =>
        {
            config.CaseWhen(1, (k, c) => $"{k}-{c}-1");
            config.Else("default-1");
        });

        var switch2 = SwitchBuilder.BuildContextual<int, string, string>(config =>
        {
            config.CaseWhen(1, (k, c) => $"{k}-{c}-2");
            config.Else("default-2");
        });

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(switch1.ThenValue(1, "ctx"), Is.EqualTo("1-ctx-1"));
            Assert.That(switch2.ThenValue(1, "ctx"), Is.EqualTo("1-ctx-2"));
            Assert.That(switch1.ThenValue(999, "ctx"), Is.EqualTo("default-1"));
            Assert.That(switch2.ThenValue(999, "ctx"), Is.EqualTo("default-2"));
        });
    }

    [Test]
    public void BuildContextual_ValueFactoryWithClosure_MaintainsCorrectState()
    {
        // Arrange
        var multiplier = 10;
        var sut = SwitchBuilder.BuildContextual<int, int, int>(config =>
        {
            config.CaseWhen(1, (key, offset) => (key * multiplier) + offset);
            config.CaseWhen(2, (key, offset) => (key * multiplier) + offset);
        });

        // Act
        var result1 = sut.ThenValue(1, 5);
        multiplier = 20;
        var result2 = sut.ThenValue(2, 10);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result1, Is.EqualTo(15)); // (1 * 10) + 5
            Assert.That(result2, Is.EqualTo(50)); // (2 * 20) + 10 - Uses updated multiplier
        });
    }

    #endregion

    #region Non-Default Implementation Tests

    [Test]
    public void BuildContextual_WithSourceSwitchFromNonDefaultImplementation_DoesNotThrow()
    {
        // Arrange
        var source = new StubContextualSwitch<int, string, string>();

        // Act
        var sut = SwitchBuilder.BuildContextual(source, config =>
        {
            config.CaseWhen(1, (k, c) => $"{k}-{c}");
            config.Else("default");
        });

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(sut.ThenValue(1, "alpha"), Is.EqualTo("1-alpha"));
            Assert.That(sut.ThenValue(999, "beta"), Is.EqualTo("default"));
        });
    }

    #endregion

    #region Null Argument Tests

    [Test]
    public void BuildContextual_WithNullBuilderFactory_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            SwitchBuilder.BuildContextual<int, string, string>(null!));
    }

    [Test]
    public void BuildContextual_WithNullSource_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            SwitchBuilder.BuildContextual<int, string, string>(null!, config => { }));
    }

    [Test]
    public void BuildContextual_WithSourceAndNullBuilderFactory_ThrowsArgumentNullException()
    {
        // Arrange
        var source = SwitchBuilder.BuildContextual<int, string, string>(config =>
        {
            config.CaseWhen(1, "one");
        });

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            SwitchBuilder.BuildContextual(source, null!));
    }

    #endregion
}

/// <summary>
/// Stub implementation of IContextualSwitch for testing BuildContextual with non-DefaultContextualSwitch sources.
/// </summary>
internal sealed class StubContextualSwitch<TKey, TContext, TValue> : IContextualSwitch<TKey, TContext, TValue>
    where TKey : notnull
{
    public Func<TKey, TContext, TValue>? Then(TKey key) => null;

    public TValue? ThenValue(TKey key, TContext context) => default;
}
