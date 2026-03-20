namespace IDCR.Abstractions.Interceptors;

public interface IAuditModifiedTimestamp
{
    DateTimeOffset? ModifiedTimestampUtc { get; set; }
}
