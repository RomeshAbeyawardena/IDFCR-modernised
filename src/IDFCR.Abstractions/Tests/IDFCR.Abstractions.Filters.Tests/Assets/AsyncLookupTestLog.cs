namespace IDFCR.Abstractions.Filters.Tests.Assets;

public sealed record AsyncLookupEvent(string Source, string Operation, object? Filter, CancellationToken CancellationToken);

public static class AsyncLookupTestLog
{
    private static readonly List<AsyncLookupEvent> EventsList = [];

    public static IReadOnlyList<AsyncLookupEvent> Events => EventsList;

    public static void Reset()
    {
        EventsList.Clear();
    }

    public static void Record(string source, string operation, object? filter, CancellationToken cancellationToken)
    {
        EventsList.Add(new AsyncLookupEvent(source, operation, filter, cancellationToken));
    }
}
