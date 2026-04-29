namespace IDFCR.TestUtilities;

/// <summary>
/// Represents a db context marker
/// </summary>
public class DbContextMarker<TDb>
{
    /// <summary>
    /// Gets or initializes the entries in the db context marker
    /// </summary>
    public List<TDb> Entries { get; init; } = [];
}
