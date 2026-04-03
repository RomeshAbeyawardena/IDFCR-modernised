using IDFCR.Abstractions.Cli;
using NUnit.Framework;

namespace IDFCR.Abstractions.Cli.Tests;

[TestFixture]
public class DefaultPromptGreeterTests
{
    private DefaultPromptGreeter greeter = null!;

    [SetUp]
    public void SetUp()
    {
        greeter = new DefaultPromptGreeter(PromptGreeterOptions.WesternDefault);
    }

    [Test]
    public void GenerateGreetingPrompt_WithNullOrWhitespacePrompt_ReturnsEmpty()
    {
        using (Assert.EnterMultipleScope())
        {
            Assert.That(greeter.GenerateGreetingPrompt(null, DateTimeOffset.Now, new Dictionary<string, string>()), Is.EqualTo(string.Empty));
            Assert.That(greeter.GenerateGreetingPrompt(string.Empty, DateTimeOffset.Now, new Dictionary<string, string>()), Is.EqualTo(string.Empty));
            Assert.That(greeter.GenerateGreetingPrompt("   ", DateTimeOffset.Now, new Dictionary<string, string>()), Is.EqualTo(string.Empty));
        }
    }

    [Test]
    public void GenerateGreetingPrompt_WithTemplateAndPlaceholders_ReplacesAllKnownTokens()
    {
        var template = "A:{A}|B:{B}|A2:{A}|Unknown:{X}";
        var result = greeter.GenerateGreetingPrompt(
            template,
            DateTimeOffset.Now,
            new Dictionary<string, string>
            {
                ["A"] = "one",
                ["B"] = "two"
            });

        Assert.That(result, Is.EqualTo("A:one|B:two|A2:one|Unknown:{X}"));
    }

    [TestCase(0, 29, "evening")]
    [TestCase(0, 30, "morning")]
    [TestCase(11, 59, "morning")]
    [TestCase(12, 0, "afternoon")]
    [TestCase(16, 59, "afternoon")]
    [TestCase(17, 0, "evening")]
    public void GenerateGreetingPrompt_UsesExpectedTimeOfDayBoundaries(int hour, int minute, string expectedLabel)
    {
        var time = CreateLocalTime(hour, minute);
        var prompt = greeter.GenerateGreetingPrompt(time, overridePrompt: "{TimeOfDay}");

        Assert.That(prompt, Is.EqualTo(expectedLabel));
    }

    [Test]
    public void GenerateGreetingPrompt_WithOverridePrompt_UsesOverrideInsteadOfDefaultTemplate()
    {
        var time = CreateLocalTime(13, 30);
        var prompt = greeter.GenerateGreetingPrompt(time, overridePrompt: "NOW={CurrentTime};TOD={TimeOfDay}");

        Assert.That(prompt, Is.EqualTo("NOW=13:30;TOD=afternoon"));
    }

    [Test]
    public void GenerateGreetingPrompt_WithOverrideOptions_UsesOriginalTemplatePlaceholder()
    {
        var options = PromptGreeterOptions.WesternDefault.Combine(new PromptGreeterOptions
        {
            DefaultPromptTemplate = "Current:{OriginalTemplate}"
        });

        var prompt = greeter.GenerateGreetingPrompt(CreateLocalTime(9, 0), options);

        Assert.That(prompt, Is.EqualTo("Current:Good morning. The current time is 09:00."));
    }

    [Test]
    public void GenerateGreetingPrompt_WhenLabelsAreNull_FallsBackToWesternDefaults()
    {
        var fallbackOptions = new PromptGreeterOptions(
            EnablePromptGreeting: true,
            MorningStartTime: new TimeOnly(0, 30),
            AfternoonStartTime: new TimeOnly(12, 0),
            EveningStartTime: new TimeOnly(17, 0))
        {
            DefaultPromptTemplate = "{TimeOfDay}",
            MorningLabel = null,
            AfternoonLabel = null,
            EveningLabel = null
        };

        var fallbackGreeter = new DefaultPromptGreeter(fallbackOptions);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(fallbackGreeter.GenerateGreetingPrompt(CreateLocalTime(8, 0)), Is.EqualTo("morning"));
            Assert.That(fallbackGreeter.GenerateGreetingPrompt(CreateLocalTime(13, 0)), Is.EqualTo("afternoon"));
            Assert.That(fallbackGreeter.GenerateGreetingPrompt(CreateLocalTime(20, 0)), Is.EqualTo("evening"));
        }
    }

    private static DateTimeOffset CreateLocalTime(int hour, int minute)
    {
        var localDateTime = new DateTime(2026, 04, 03, hour, minute, 0, DateTimeKind.Local);
        return new DateTimeOffset(localDateTime);
    }
}