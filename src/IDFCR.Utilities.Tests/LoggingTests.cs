using IDFCR.Utilities.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using Moq;
using NUnit.Framework;
using System.Reflection.Metadata;
using System.Text;

namespace IDFCR.Utilities.Tests;

public class TestLogger : ILogger
{
    private readonly StringBuilder loggerBuilder = new();

    /// <inheritdoc />
    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        throw new NotImplementedException("Implement me if you need me ");
    }

    /// <inheritdoc />
    public bool IsEnabled(LogLevel logLevel)
    {
        return true;
    }

    /// <inheritdoc />
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        var message = formatter(state, exception);
        loggerBuilder.AppendLine($"{logLevel}: {message}");
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return loggerBuilder.ToString();
    }
}

[TestFixture]
internal class LoggingTests
{
    TestLogger testLogger;
    [SetUp]
    public void SetUp()
    {
        testLogger = new();
    }

    [Test]
    public void TestThatSomethingHappens()
    {
        string bob = "tob";
        string tub = "mot";
        
        testLogger.LogMethod(LogLevel.Information, "Test {bob} {tub}", args: [bob, tub]);
        var value = testLogger.ToString();
        Assert.That(value, Is.EqualTo($"Information: {nameof(TestThatSomethingHappens)}: Test tob mot{Environment.NewLine}"));
    }
}
