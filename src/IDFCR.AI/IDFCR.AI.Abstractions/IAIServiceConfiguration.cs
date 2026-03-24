namespace IDFCR.AI.Abstractions;

public interface IAIServiceConfiguration
{
    string ServiceName { get; }

    TimeSpan? Timeout { get; }
}
