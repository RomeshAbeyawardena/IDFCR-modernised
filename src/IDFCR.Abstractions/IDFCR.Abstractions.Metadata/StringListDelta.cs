namespace IDFCR.Abstractions.Metadata;

/// <summary>
/// Represents a delta (change) in a list of strings, containing items to add and remove.
/// </summary>
public record StringListDelta : IStringListDelta
{
    /// <summary>
    /// Gets the collection of strings to be added to the list.
    /// </summary>
    public IEnumerable<string> Add { get; init; } = [];
    /// <summary>
    /// Gets the collection of strings to be removed from the list.
    /// </summary>
    public IEnumerable<string> Remove { get; init; } = [];
}
