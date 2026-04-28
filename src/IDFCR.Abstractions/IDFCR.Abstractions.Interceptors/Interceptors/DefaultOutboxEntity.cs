using IDFCR.Abstractions.Interceptors.Handlers;

namespace IDFCR.Abstractions.Interceptors.Interceptors;

internal class DefaultOutboxEntity : IOutboxEntity
{
    public string? Data { get; set; }
    public DateTimeOffset? CompletedTimestampUtc { get; set; }
    public DateTimeOffset? FailedTimestampUtc { get; set; }
    public DateTimeOffset? AcknowledgedTimestampUtc { get; set; }
    public DateTimeOffset CreatedTimestampUtc { get; set; }
    public DateTimeOffset? ModifiedTimestampUtc { get; set; }
    public object? Id { get; set; }
}
