namespace IDFCR.Abstractions.Metadata;

/// <summary>
/// Represents the naming convention for metadata keys used in unit results and other related types. This interface allows for customization of the keys used to store metadata, success status, failure reasons, error messages, actions, current entity state, and paging information. By implementing this interface, users can define their own naming conventions for these keys instead of using the default ones defined in the <see cref="Meta"/> class.
/// </summary>
public interface IMetaNamingConvention
{
    /// <summary>
    /// Gets the key used to store metadata in unit results and other related types. This key is used to identify the metadata section in the results, allowing for consistent access to metadata information across different implementations. By default, it returns the value defined in <see cref="Meta.Key"/>, but it can be customized to use a different key if needed.
    /// </summary>
    string Key { get; }
    /// <summary>
    /// Gets the key used to store items in unit results and other related types. This key is used to identify the items section in the results, allowing for consistent access to item information across different implementations. By default, it returns the value defined in <see cref="Meta.ItemKey"/>, but it can be customized to use a different key if needed.
    /// </summary>
    string ItemKey { get; }
    /// <summary>
    /// Gets the key used to store the success status in unit results and other related types. This key is used to identify whether an operation was successful or not, allowing for consistent access to success information across different implementations. By default, it returns the value defined in <see cref="Meta.SuccessKey"/>, but it can be customized to use a different key if needed.
    /// </summary>
    string SuccessKey { get; }
    /// <summary>
    /// Gets the key used to store the failure reason in unit results and other related types. This key is used to identify the reason for a failure when an operation is not successful, allowing for consistent access to failure information across different implementations. By default, it returns the value defined in <see cref="Meta.FailureReason"/>, but it can be customized to use a different key if needed.
    /// </summary>
    string FailureReason { get; }
    /// <summary>
    /// Gets the key used to store the error message in unit results and other related types. This key is used to identify the error message associated with a failure when an operation is not successful, allowing for consistent access to error information across different implementations. By default, it returns the value defined in <see cref="Meta.ErrorMessage"/>, but it can be customized to use a different key if needed.
    /// </summary>
    string ErrorMessage { get; }
    /// <summary>
    /// Gets the key used to store the action associated with a unit result or other related types. This key is used to identify the action that was performed, allowing for consistent access to action information across different implementations. By default, it returns the value defined in <see cref="Meta.ActionKey"/>, but it can be customized to use a different key if needed.
    /// </summary>
    string ActionKey { get; }
    /// <summary>
    /// Gets the key used to store the current entity state in unit results and other related types. This key is used to identify the state of the entity after an operation has been performed, allowing for consistent access to entity state information across different implementations. By default, it returns the value defined in <see cref="Meta.CurrentEntityState"/>, but it can be customized to use a different key if needed.
    /// </summary>
    string CurrentEntityState { get; }
    /// <summary>
    /// Gets the keys used for paging information in paged query results. This property returns an instance of <see cref="IMetaPagingNamingConvention"/>, which defines the keys used to store paging information such as total rows, total pages, page index, and page size. By default, it returns an instance of <see cref="DefaultMetaPagingNamingConvention"/>, which uses the keys defined in <see cref="Meta.Paging"/>, but it can be customized to use different keys if needed.
    /// </summary>

    IMetaPagingNamingConvention Paging { get; }
}
