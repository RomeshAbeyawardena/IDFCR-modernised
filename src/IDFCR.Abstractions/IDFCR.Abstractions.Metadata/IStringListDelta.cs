namespace IDFCR.Abstractions.Metadata;

/// <summary>
/// Represents a delta (change) in a list of strings, containing items to add and remove.
/// </summary>
public interface IStringListDelta
{
    /// <summary>
    /// Gets the collection of strings to be added to the list.
    /// </summary>
    IEnumerable<string> Add { get; }
    /// <summary>
    /// Gets the collection of strings to be removed from the list.
    /// </summary>
    IEnumerable<string> Remove { get; }
}
