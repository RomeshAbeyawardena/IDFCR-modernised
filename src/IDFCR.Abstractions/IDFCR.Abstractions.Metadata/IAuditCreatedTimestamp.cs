namespace IDFCR.Abstractions.Metadata;

public interface IAuditCreatedTimestamp
{
    DateTimeOffset CreatedTimestampUtc { get; set; }
}
