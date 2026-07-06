namespace IDFCR.Abstractions.Outbox;

internal class DefaultOutboxEntity : IOutboxEntity
{
    public string EntityType { get; set; } = null!;
    public string? Data { get; set; }
    public DateTimeOffset? CompletedTimestampUtc { get; set; }
    public DateTimeOffset? FailedTimestampUtc { get; set; }
    public DateTimeOffset? ProcessedTimestampUtc { get; set; }
    public DateTimeOffset CreatedTimestampUtc { get; set; }
    public DateTimeOffset? ModifiedTimestampUtc { get; set; }
    public object? Id { get; set; }
}
