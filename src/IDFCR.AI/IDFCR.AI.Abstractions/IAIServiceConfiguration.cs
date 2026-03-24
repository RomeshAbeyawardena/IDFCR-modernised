namespace IDFCR.AI.Abstractions;

/// <summary>
/// Describes the configuration required by an <see cref="IAIService"/> implementation.
/// </summary>
public interface IAIServiceConfiguration
{
    /// <summary>
    /// Gets the logical name of the AI service.
    /// </summary>
    string ServiceName { get; }

    /// <summary>
    /// Gets the optional timeout applied to AI requests.
    /// </summary>
    TimeSpan? Timeout { get; }
}
