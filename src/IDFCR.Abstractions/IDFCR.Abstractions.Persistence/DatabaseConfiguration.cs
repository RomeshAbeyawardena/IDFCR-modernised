namespace IDFCR.Abstractions.Persistence;

/// <summary>
/// Configures repository behavior for database-backed persistence.
/// </summary>
public class DatabaseConfiguration
{
    /// <summary>
    /// Gets or sets whether soft deletion is enabled.
    /// </summary>
    public bool UseSoftDeletion { get; set; }
}
