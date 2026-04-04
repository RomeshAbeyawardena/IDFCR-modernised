using IDFCR.Abstractions.Cli.Formatters;
using IDFCR.Abstractions.Cli.ManagedStreams;
using IDFCR.TestUtilities;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace IDFCR.Abstractions.Cli.Tests;

[TestFixture]
internal class ManagedStreamTableFormatterTests
{
    private StringReader sr;
    private StringWriter sw;
    private StringWriter esw;
    private ManagedStreamTableFormatter formatter;
    private Mock<IManagedStream> mockManagedStream;
    [SetUp]
    public void SetUp()
    {
        mockManagedStream = new Mock<IManagedStream>();
        sr = new(string.Empty);
        sw = new();
        esw = new();
        mockManagedStream.SetupGet(m => m.In).Returns(new StringReadableStream(sr));
        mockManagedStream.SetupGet(m => m.Out).Returns(new StringWriteableStream(sw));
        mockManagedStream.SetupGet(m => m.Error).Returns(new StringWriteableStream(esw));
        
        formatter = new ManagedStreamTableFormatter(mockManagedStream.Object);
    }

    [TearDown]
    public void TearDown()
    {
        formatter?.Dispose();
        sr.Dispose();
        sw.Dispose();
        esw.Dispose();
    }

    [Test]
    public async Task Test()
    {
        formatter.Target = ManagedStreamTarget.Out;
        object notAnArray = new();
        Assert.ThrowsAsync<ArgumentException>(async () => await formatter.FormatAsync<object>(notAnArray, CancellationToken.None));

        var fruits = new[] { "Apple", "Banana", "Cherry" };
        await formatter.FormatAsync(fruits, CancellationToken.None);
    }
}
