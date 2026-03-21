using IDFCR.Abstractions.Metadata;

namespace IDFCR.Abstractions.Interceptors.Tests.Assets;

internal record Customer : IAuditCreatedTimestamp, IAuditModifiedTimestamp
{
    public DateTimeOffset CreatedTimestampUtc { get; set; }
    public DateTimeOffset? ModifiedTimestampUtc { get; set; }
}
