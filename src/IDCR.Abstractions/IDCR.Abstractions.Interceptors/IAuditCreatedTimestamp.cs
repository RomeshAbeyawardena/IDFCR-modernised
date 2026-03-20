namespace IDCR.Abstractions.Interceptors;

public interface IAuditCreatedTimestamp
{
    DateTimeOffset CreatedTimestampUtc { get; set; }
}
