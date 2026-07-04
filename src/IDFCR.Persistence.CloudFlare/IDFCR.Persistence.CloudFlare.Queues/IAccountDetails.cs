namespace IDFCR.Persistence.CloudFlare.Queues;

/// <summary>
/// Represents the details required to access a Cloudflare account and interact with its queues. This interface defines the necessary properties for account access and queue operations, including the API token, account ID, queue ID or name, and API version. Implementing this interface allows for consistent handling of account details across different components that interact with Cloudflare queues.
/// </summary>
public interface IAccountDetails
{
    /// <summary>
    /// Gets the API token used for authenticating requests to the Cloudflare API. This token is required for accessing the account and performing operations on its queues. The ApiToken property should be securely stored and managed, as it provides access to sensitive account information and operations.
    /// </summary>
    string? ApiToken { get; init; }
    /// <summary>
    /// Gets the unique identifier of the Cloudflare account. This property is used to specify which account the application is interacting with when making API requests. The AccountId is essential for ensuring that operations are performed on the correct account and its associated resources, such as queues.
    /// </summary>
    string? AccountId { get; init; }
    /// <summary>
    /// Gets the identifier or name of the specific queue within the Cloudflare account that the application will interact with. This property is used to specify which queue to access for operations such as pulling messages, adding messages, or managing queue settings. The QueueIdOrName property allows for flexibility in identifying the queue by either its unique ID or its human-readable name.
    /// </summary>
    string? QueueIdOrName { get; init; }
    /// <summary>
    /// Gets the version of the Cloudflare API being used. This property is important for ensuring compatibility with the API endpoints and features available in that specific version. Implementations of this interface can specify the API version to be used for requests, allowing for flexibility in interacting with different versions of the Cloudflare API.
    /// </summary>
    string? ApiVersion { get; init; }
}