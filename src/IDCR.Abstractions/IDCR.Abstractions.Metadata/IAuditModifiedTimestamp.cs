namespace IDCR.Abstractions.Metadata;

public interface IAuditModifiedTimestamp
{
    DateTimeOffset? ModifiedTimestampUtc { get; set; }
}
