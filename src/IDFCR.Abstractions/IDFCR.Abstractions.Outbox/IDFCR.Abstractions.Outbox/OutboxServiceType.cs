namespace IDFCR.Abstractions.Outbox;

public enum OutboxServiceType
{
    Unknown,
    Dispatcher,
    Pipeline,
    Publisher,
    Reader
}
