namespace IDFCR.Abstractions.Metadata;

/// <summary>
/// Defines the naming convention for metadata keys used in unit results and other related types. This allows for customization of the keys used to store metadata, success status, failure reasons, error messages, actions, current entity state, and paging information. By default, it uses the keys defined in the <see cref="Meta"/> class, but it can
/// </summary>
public static class MetaNaming
{
    private static IMetaNamingConvention _metaNamingConvention = new DefaultMetaNamingConvention();
    /// <summary>
    /// Gets or sets the naming convention for metadata keys. If not set, it defaults to the keys defined in the <see cref="Meta"/> class.
    /// </summary>
    public static IMetaNamingConvention Convention { 
        get => _metaNamingConvention ?? new DefaultMetaNamingConvention();
        set {
            ArgumentNullException.ThrowIfNull(value);
            _metaNamingConvention = value;
        }
    }
}
