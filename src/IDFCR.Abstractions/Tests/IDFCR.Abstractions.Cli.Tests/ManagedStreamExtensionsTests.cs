using IDFCR.Abstractions.Cli.Extensions;
using IDFCR.Abstractions.Cli.ManagedStreams;
using IDFCR.Abstractions.Results;
using IDFCR.TestUtilities;
using Moq;
using NUnit.Framework;

namespace IDFCR.Abstractions.Cli.Tests;

[TestFixture]
internal class ManagedStreamExtensionsTests
{
    private Mock<IManagedStream> managedStream = null!;
    private StringReader input = null!;
    private StringWriter output = null!;
    private StringWriter error = null!;

    [SetUp]
    public void SetUp()
    {
        input = new StringReader(string.Empty);
        output = new StringWriter();
        error = new StringWriter();

        managedStream = new Mock<IManagedStream>();
        managedStream.SetupGet(x => x.In).Returns(new StringReadableStream(input));
        managedStream.SetupGet(x => x.Out).Returns(new StringWriteableStream(output));
        managedStream.SetupGet(x => x.Error).Returns(new StringWriteableStream(error));
        managedStream.SetupGet(x => x.Width).Returns(12);
    }

    [TearDown]
    public void TearDown()
    {
        input.Dispose();
        output.Dispose();
        error.Dispose();
    }

    [Test]
    public async Task PromptAsync_WritesPromptAndReturnsInputLine()
    {
        input = new StringReader("romesh");
        managedStream.SetupGet(x => x.In).Returns(new StringReadableStream(input));

        var response = await managedStream.Object.PromptAsync("Name", CancellationToken.None);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(response, Is.EqualTo("romesh"));
            Assert.That(output.ToString(), Is.EqualTo("Name: "));
        }
    }

    [Test]
    public async Task DisplayPaged_WhenResultIsFailure_WritesErrorAndNoStandardOutput()
    {
        var result = CreatePagedResult<string>(
            isSuccess: false,
            rows: null,
            totalRows: 0,
            exception: new InvalidOperationException("boom"));

        await managedStream.Object.DisplayPaged(
            result,
            map: x => x,
            formatData: x => x,
            cancellationToken: CancellationToken.None);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(error.ToString(), Is.EqualTo($"Error retrieving data: boom{Environment.NewLine}"));
            Assert.That(output.ToString(), Is.Empty);
        }
    }

    [Test]
    public async Task DisplayPaged_WhenResultHasNoRows_WritesNoRowsMessage()
    {
        var result = CreatePagedResult(
            isSuccess: true,
            rows: Array.Empty<string>(),
            totalRows: 0);

        await managedStream.Object.DisplayPaged(
            result,
            map: x => x,
            formatData: x => x,
            cancellationToken: CancellationToken.None);

        Assert.That(output.ToString(), Is.EqualTo($"No rows returns found{Environment.NewLine}"));
    }

    [Test]
    public async Task DisplayPaged_WhenResultHasRows_WritesFormattedRowsAndSummary()
    {
        var result = CreatePagedResult(
            isSuccess: true,
            rows: new[] { "alpha", "beta" },
            totalRows: 7);

        await managedStream.Object.DisplayPaged(
            result,
            map: x => x,
            formatData: x => $"- {x}",
            cancellationToken: CancellationToken.None);

        var expected =
            $"- alpha{Environment.NewLine}" +
            $"- beta{Environment.NewLine}" +
            $"------------7 found. Displaying 2 items{Environment.NewLine}";

        Assert.That(output.ToString(), Is.EqualTo(expected));
    }

    [Test]
    public async Task DisplayPagedTable_WhenResultHasRows_WritesTableSkeletonRowsAndSummary()
    {
        var rows = new[]
        {
            new MigrationRow("MigrationA"),
            new MigrationRow("MigrationB")
        };

        var result = CreatePagedResult(
            isSuccess: true,
            rows: rows,
            totalRows: 2);

        await managedStream.Object.DisplayPagedTable(
            result,
            map: x => x,
            cancellationToken: CancellationToken.None,
            new TableField<MigrationRow>
            {
                Field = x => x.Name,
                Title = "Migration",
                RowWidth = 10
            });

        var expected =
            $"------------{Environment.NewLine}" +
            $"Migration {Environment.NewLine}" +
            $"------------{Environment.NewLine}" +
            $"MigrationA{Environment.NewLine}" +
            $"MigrationB{Environment.NewLine}" +
            $"------------2 found. Displaying 2 items{Environment.NewLine}";

        Assert.That(output.ToString(), Is.EqualTo(expected));
    }

    [Test]
    public async Task DisplayPagedTable_WhenResultHasNoRows_StillWritesTableSkeletonThenNoRowsMessage()
    {
        var result = CreatePagedResult(
            isSuccess: true,
            rows: Array.Empty<MigrationRow>(),
            totalRows: 0);

        await managedStream.Object.DisplayPagedTable(
            result,
            map: x => x,
            cancellationToken: CancellationToken.None,
            new TableField<MigrationRow>
            {
                Field = x => x.Name,
                Title = "Migration",
                RowWidth = 10
            });

        var expected =
            $"------------{Environment.NewLine}" +
            $"Migration {Environment.NewLine}" +
            $"------------{Environment.NewLine}" +
            $"No rows returns found{Environment.NewLine}";

        Assert.That(output.ToString(), Is.EqualTo(expected));
    }

    private static IUnitPagedResult<T> CreatePagedResult<T>(
        bool isSuccess,
        IEnumerable<T>? rows,
        int totalRows,
        Exception? exception = null)
    {
        return UnitPagedResult.FromResult(
            result: rows,
            totalRows: totalRows,
            pagedQuery: new PagedQuery(pageSize: 25, pageIndex: 0),
            isSuccess: isSuccess,
            exception: exception);
    }

    private sealed record MigrationRow(string Name);
}