namespace IDFCR.TestUtilities;

/// <summary>
/// Represents a db context marker
/// </summary>
public class DbContextMarker<TDb>
{
    public List<TDb> Entries { get; init; } = [];
}
