
using NUnit.Framework;

namespace IDFCR.Utilities.Tests;

[TestFixture]
internal class SwitchTests
{
    private static readonly string[] Values = ["foo", "bar", "roo", "moo", "two"];
    private static string GetValue(int index)
    {
        return Values[index];
    }
    private const string InvalidOptionMessage = "Invalid option.";
    private ISwitch<int, string> sut;
    [SetUp]
    public void SetUp()
    {
        sut = SwitchBuilder.Build<int, string>(config =>
        {
            for (int i = 0; i < Values.Length; i++)
            {
                config.CaseWhen(i, GetValue);
            }

            config.Else((key) => InvalidOptionMessage);
        });
    }

    [Test]
    public void Test()
    {
        for (int i = 0; i < 5; i++)
        {
            var m = Values[i];
            Assert.That(sut.ThenValue(i), Is.EqualTo(m));
        }

        Assert.That(sut.ThenValue(-1), Is.EqualTo(InvalidOptionMessage));
    }
}

[TestFixture]
internal class SwitchBuilderTests
{
    #region Basic Switch Creation Tests

    [Test]
    public void Build_WithSimpleValueCases_ReturnsCorrectValues()
    {
        // Arrange & Act
        var sut = SwitchBuilder.Build<int, string>(config =>
        {
            config.CaseWhen(1, "one");
            config.CaseWhen(2, "two");
            config.CaseWhen(3, "three");
        });

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(sut.ThenValue(1), Is.EqualTo("one"));
            Assert.That(sut.ThenValue(2), Is.EqualTo("two"));
            Assert.That(sut.ThenValue(3), Is.EqualTo("three"));
        });
    }

    [Test]
    public void Build_WithValueFactoryCases_ReturnsCorrectValues()
    {
        // Arrange & Act
        var sut = SwitchBuilder.Build<int, string>(config =>
        {
            config.CaseWhen(1, key => $"Value: {key}");
            config.CaseWhen(2, key => $"Value: {key}");
            config.CaseWhen(3, key => $"Value: {key}");
        });

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(sut.ThenValue(1), Is.EqualTo("Value: 1"));
            Assert.That(sut.ThenValue(2), Is.EqualTo("Value: 2"));
            Assert.That(sut.ThenValue(3), Is.EqualTo("Value: 3"));
        });
    }

    [Test]
    public void Build_WithNoMatchingCase_ReturnsDefault()
    {
        // Arrange
        var sut = SwitchBuilder.Build<int, string>(config =>
        {
            config.CaseWhen(1, "one");
            config.CaseWhen(2, "two");
        });

        // Act
        var result = sut.ThenValue(999);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public void Build_WithEmptyConfiguration_ReturnsDefaultTValueForAnyKey()
    {
        // Arrange
        var sut = SwitchBuilder.Build<string, int>(config => { });

        // Act & Assert
        // Returns default(TValue) which is 0 for int - indistinguishable from a valid 0 value
        Assert.Multiple(() =>
        {
            Assert.That(sut.ThenValue("any"), Is.EqualTo(default(int)));
            Assert.That(sut.ThenValue("key"), Is.EqualTo(default(int)));
        });
    }

    #endregion

    #region Default Case (Else) Tests

    [Test]
    public void Build_WithElseValue_ReturnsElseValueForNonMatchingKey()
    {
        // Arrange
        var sut = SwitchBuilder.Build<int, string>(config =>
        {
            config.CaseWhen(1, "one");
            config.CaseWhen(2, "two");
            config.Else("default");
        });

        // Act & Assert
        Assert.Multiple(() =>
        {
            Assert.That(sut.ThenValue(1), Is.EqualTo("one"));
            Assert.That(sut.ThenValue(999), Is.EqualTo("default"));
            Assert.That(sut.ThenValue(-1), Is.EqualTo("default"));
        });
    }

    [Test]
    public void Build_WithElseValueFactory_ReturnsElseValueForNonMatchingKey()
    {
        // Arrange
        var sut = SwitchBuilder.Build<int, string>(config =>
        {
            config.CaseWhen(1, "one");
            config.CaseWhen(2, "two");
            config.Else(key => $"Unknown key: {key}");
        });

        // Act & Assert
        Assert.Multiple(() =>
        {
            Assert.That(sut.ThenValue(1), Is.EqualTo("one"));
            Assert.That(sut.ThenValue(999), Is.EqualTo("Unknown key: 999"));
            Assert.That(sut.ThenValue(-1), Is.EqualTo("Unknown key: -1"));
        });
    }

    [Test]
    public void Build_ElseCalledMultipleTimes_UsesLastElseValue()
    {
        // Arrange
        var sut = SwitchBuilder.Build<int, string>(config =>
        {
            config.CaseWhen(1, "one");
            config.Else("first default");
            config.Else("second default");
            config.Else("third default");
        });

        // Act
        var result = sut.ThenValue(999);

        // Assert
        Assert.That(result, Is.EqualTo("third default"));
    }

    #endregion

    #region Case Replacement Tests

    [Test]
    public void Build_CaseDefinedMultipleTimes_UsesLastDefinedValue()
    {
        // Arrange
        var sut = SwitchBuilder.Build<int, string>(config =>
        {
            config.CaseWhen(1, "first");
            config.CaseWhen(1, "second");
            config.CaseWhen(1, "third");
        });

        // Act
        var result = sut.ThenValue(1);

        // Assert
        Assert.That(result, Is.EqualTo("third"));
    }

    [Test]
    public void Build_CaseReplacedWithValueFactory_UsesLatestFactory()
    {
        // Arrange
        var counter = 0;
        var sut = SwitchBuilder.Build<int, string>(config =>
        {
            config.CaseWhen(1, "static value");
            config.CaseWhen(1, key => $"dynamic {++counter}");
        });

        // Act
        var result1 = sut.ThenValue(1);
        var result2 = sut.ThenValue(1);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result1, Is.EqualTo("dynamic 1"));
            Assert.That(result2, Is.EqualTo("dynamic 2"));
        });
    }

    #endregion

    #region Then Method Tests

    [Test]
    public void Then_WithMatchingKey_ReturnsValueFactory()
    {
        // Arrange
        var sut = SwitchBuilder.Build<int, string>(config =>
        {
            config.CaseWhen(1, key => $"Value: {key}");
        });

        // Act
        var factory = sut.Then(1);

        // Assert
        Assert.That(factory, Is.Not.Null);
        Assert.That(factory!(1), Is.EqualTo("Value: 1"));
    }

    [Test]
    public void Then_WithNonMatchingKeyAndNoElse_ReturnsNull()
    {
        // Arrange
        var sut = SwitchBuilder.Build<int, string>(config =>
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
        var sut = SwitchBuilder.Build<int, string>(config =>
        {
            config.CaseWhen(1, "one");
            config.Else(key => $"Default: {key}");
        });

        // Act
        var factory = sut.Then(999);

        // Assert
        Assert.That(factory, Is.Not.Null);
        Assert.That(factory!(999), Is.EqualTo("Default: 999"));
    }

    #endregion

    #region Source-Based Build Tests

    [Test]
    public void Build_WithSourceSwitch_PreservesExistingCases()
    {
        // Arrange
        var source = SwitchBuilder.Build<int, string>(config =>
        {
            config.CaseWhen(1, "one");
            config.CaseWhen(2, "two");
            config.Else("default");
        });

        // Act
        var extended = SwitchBuilder.Build(source, config =>
        {
            config.CaseWhen(3, "three");
        });

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(extended.ThenValue(1), Is.EqualTo("one"));
            Assert.That(extended.ThenValue(2), Is.EqualTo("two"));
            Assert.That(extended.ThenValue(3), Is.EqualTo("three"));
            Assert.That(extended.ThenValue(999), Is.EqualTo("default"));
        });
    }

    [Test]
    public void Build_WithSourceSwitch_CanOverrideExistingCases()
    {
        // Arrange
        var source = SwitchBuilder.Build<int, string>(config =>
        {
            config.CaseWhen(1, "one");
            config.CaseWhen(2, "two");
        });

        // Act
        var modified = SwitchBuilder.Build(source, config =>
        {
            config.CaseWhen(1, "ONE");
            config.CaseWhen(2, "TWO");
        });

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(modified.ThenValue(1), Is.EqualTo("ONE"));
            Assert.That(modified.ThenValue(2), Is.EqualTo("TWO"));
        });
    }

    [Test]
    public void Build_WithSourceSwitch_CanReplaceElseClause()
    {
        // Arrange
        var source = SwitchBuilder.Build<int, string>(config =>
        {
            config.CaseWhen(1, "one");
            config.Else("original default");
        });

        // Act
        var modified = SwitchBuilder.Build(source, config =>
        {
            config.Else("new default");
        });

        // Assert
        Assert.That(modified.ThenValue(999), Is.EqualTo("new default"));
    }

    [Test]
    public void Build_WithSourceSwitch_OriginalSwitchRemainsUnchanged()
    {
        // Arrange
        var source = SwitchBuilder.Build<int, string>(config =>
        {
            config.CaseWhen(1, "one");
            config.Else("original");
        });

        // Act
        var modified = SwitchBuilder.Build(source, config =>
        {
            config.CaseWhen(1, "ONE");
            config.CaseWhen(2, "TWO");
            config.Else("modified");
        });

        // Assert - verify source is unchanged
        Assert.Multiple(() =>
        {
            Assert.That(source.ThenValue(1), Is.EqualTo("one"));
            Assert.That(source.ThenValue(2), Is.EqualTo("original")); // Falls through to else since key 2 not defined
            Assert.That(source.ThenValue(999), Is.EqualTo("original"));
        });

        // Assert - verify modified is correct
        Assert.Multiple(() =>
        {
            Assert.That(modified.ThenValue(1), Is.EqualTo("ONE"));
            Assert.That(modified.ThenValue(2), Is.EqualTo("TWO"));
            Assert.That(modified.ThenValue(999), Is.EqualTo("modified"));
        });
    }

    #endregion

    #region Method Chaining Tests

    [Test]
    public void CaseWhen_ReturnsBuilder_AllowsMethodChaining()
    {
        // Arrange & Act
        var sut = SwitchBuilder.Build<int, string>(config =>
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
            Assert.That(sut.ThenValue(1), Is.EqualTo("one"));
            Assert.That(sut.ThenValue(2), Is.EqualTo("two"));
            Assert.That(sut.ThenValue(3), Is.EqualTo("three"));
        });
    }

    #endregion

    #region Different Key Type Tests

    [Test]
    public void Build_WithStringKeys_WorksCorrectly()
    {
        // Arrange
        var sut = SwitchBuilder.Build<string, int>(config =>
        {
            config.CaseWhen("one", 1);
            config.CaseWhen("two", 2);
            config.CaseWhen("three", 3);
            config.Else(-1);
        });

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(sut.ThenValue("one"), Is.EqualTo(1));
            Assert.That(sut.ThenValue("two"), Is.EqualTo(2));
            Assert.That(sut.ThenValue("three"), Is.EqualTo(3));
            Assert.That(sut.ThenValue("unknown"), Is.EqualTo(-1));
        });
    }

    [Test]
    public void Build_WithGuidKeys_WorksCorrectly()
    {
        // Arrange
        var guid1 = Guid.NewGuid();
        var guid2 = Guid.NewGuid();
        var guid3 = Guid.NewGuid();

        var sut = SwitchBuilder.Build<Guid, string>(config =>
        {
            config.CaseWhen(guid1, "first");
            config.CaseWhen(guid2, "second");
            config.CaseWhen(guid3, "third");
        });

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(sut.ThenValue(guid1), Is.EqualTo("first"));
            Assert.That(sut.ThenValue(guid2), Is.EqualTo("second"));
            Assert.That(sut.ThenValue(guid3), Is.EqualTo("third"));
            Assert.That(sut.ThenValue(Guid.NewGuid()), Is.Null);
        });
    }

    [Test]
    public void Build_WithEnumKeys_WorksCorrectly()
    {
        // Arrange
        var sut = SwitchBuilder.Build<DayOfWeek, string>(config =>
        {
            config.CaseWhen(DayOfWeek.Monday, "Start of work week");
            config.CaseWhen(DayOfWeek.Friday, "End of work week");
            config.CaseWhen(DayOfWeek.Saturday, "Weekend");
            config.Else("Another day");
        });

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(sut.ThenValue(DayOfWeek.Monday), Is.EqualTo("Start of work week"));
            Assert.That(sut.ThenValue(DayOfWeek.Friday), Is.EqualTo("End of work week"));
            Assert.That(sut.ThenValue(DayOfWeek.Saturday), Is.EqualTo("Weekend"));
            Assert.That(sut.ThenValue(DayOfWeek.Tuesday), Is.EqualTo("Another day"));
        });
    }

    #endregion

    #region Complex Value Tests

    [Test]
    public void Build_WithComplexValueTypes_WorksCorrectly()
    {
        // Arrange
        var sut = SwitchBuilder.Build<int, List<string>>(config =>
        {
            config.CaseWhen(1, new List<string> { "one" });
            config.CaseWhen(2, new List<string> { "two", "items" });
            config.Else(key => new List<string> { "default", key.ToString() });
        });

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(sut.ThenValue(1), Has.Count.EqualTo(1));
            Assert.That(sut.ThenValue(1)![0], Is.EqualTo("one"));
            Assert.That(sut.ThenValue(2), Has.Count.EqualTo(2));
            Assert.That(sut.ThenValue(999), Has.Count.EqualTo(2));
            Assert.That(sut.ThenValue(999)![1], Is.EqualTo("999"));
        });
    }

    [Test]
    public void Build_WithNullableValueTypes_WorksCorrectly()
    {
        // Arrange
        var sut = SwitchBuilder.Build<int, int?>(config =>
        {
            config.CaseWhen(1, 100);
            config.CaseWhen(2, (int?)null);
            config.Else(key => key * 10);
        });

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(sut.ThenValue(1), Is.EqualTo(100));
            Assert.That(sut.ThenValue(2), Is.Null);
            Assert.That(sut.ThenValue(5), Is.EqualTo(50));
        });
    }

    #endregion

    #region Large Scale Tests

    [Test]
    public void Build_WithManyKeys_ResolvesExpectedValues()
    {
        // Arrange
        const int keyCount = 1000;
        var sut = SwitchBuilder.Build<int, string>(config =>
        {
            for (int i = 0; i < keyCount; i++)
            {
                config.CaseWhen(i, $"value-{i}");
            }
            config.Else("not found");
        });

        // Act & Assert
        Assert.Multiple(() =>
        {
            Assert.That(sut.ThenValue(0), Is.EqualTo("value-0"));
            Assert.That(sut.ThenValue(500), Is.EqualTo("value-500"));
            Assert.That(sut.ThenValue(999), Is.EqualTo("value-999"));
            Assert.That(sut.ThenValue(1000), Is.EqualTo("not found"));
        });
    }

    #endregion

    #region State Isolation Tests

    [Test]
    public void Build_MultipleSwitches_AreIndependent()
    {
        // Arrange & Act
        var switch1 = SwitchBuilder.Build<int, string>(config =>
        {
            config.CaseWhen(1, "one-1");
            config.Else("default-1");
        });

        var switch2 = SwitchBuilder.Build<int, string>(config =>
        {
            config.CaseWhen(1, "one-2");
            config.Else("default-2");
        });

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(switch1.ThenValue(1), Is.EqualTo("one-1"));
            Assert.That(switch2.ThenValue(1), Is.EqualTo("one-2"));
            Assert.That(switch1.ThenValue(999), Is.EqualTo("default-1"));
            Assert.That(switch2.ThenValue(999), Is.EqualTo("default-2"));
        });
    }

    [Test]
    public void Build_ValueFactoryWithClosure_MaintainsCorrectState()
    {
        // Arrange
        var multiplier = 10;
        var sut = SwitchBuilder.Build<int, int>(config =>
        {
            config.CaseWhen(1, key => key * multiplier);
            config.CaseWhen(2, key => key * multiplier);
        });

        // Act
        var result1 = sut.ThenValue(1);
        multiplier = 20;
        var result2 = sut.ThenValue(2);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result1, Is.EqualTo(10));
            Assert.That(result2, Is.EqualTo(40)); // Uses updated multiplier
        });
    }

    #endregion

    #region Non-Default Implementation Tests

    [Test]
    public void Build_WithSourceSwitchFromNonDefaultImplementation_DoesNotThrow()
    {
        // Arrange
        var source = new StubSwitch<int, string>();

        // Act
        var sut = SwitchBuilder.Build(source, config =>
        {
            config.CaseWhen(1, "one");
            config.Else("default");
        });

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(sut.ThenValue(1), Is.EqualTo("one"));
            Assert.That(sut.ThenValue(999), Is.EqualTo("default"));
        });
    }

    #endregion

    #region Null Argument Tests

    [Test]
    public void Build_WithNullBuilderFactory_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            SwitchBuilder.Build<int, string>(null!));
    }

    [Test]
    public void Build_WithNullSource_ThrowsArgumentNullException()
    { 
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            SwitchBuilder.Build<int, string>(null!, config => { }));
    }

    [Test]
    public void Build_WithSourceAndNullBuilderFactory_ThrowsArgumentNullException()
    {
        // Arrange
        var source = SwitchBuilder.Build<int, string>(config =>
        {
            config.CaseWhen(1, "one");
        });

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            SwitchBuilder.Build(source, null!));
    }

    #endregion
}

/// <summary>
/// Stub implementation of ISwitch for testing Build with non-DefaultSwitch sources.
/// </summary>
internal sealed class StubSwitch<TKey, TValue> : ISwitch<TKey, TValue>
    where TKey : notnull
{
    public Func<TKey, TValue>? Then(TKey key) => null;

    public TValue? ThenValue(TKey key) => default;
}
