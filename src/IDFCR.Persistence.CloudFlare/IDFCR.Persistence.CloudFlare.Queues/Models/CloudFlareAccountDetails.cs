namespace IDFCR.Persistence.CloudFlare.Queues.Models;

/// <summary>
/// Represents the details required to access a Cloudflare account and interact with its queues. This record includes properties for the API version, API token, account ID, and the specific queue ID or name. It implements the IAccountDetails interface, which defines the necessary information for account access and queue operations. The ApiVersion property is initialized to "v4" by default, indicating the version of the Cloudflare API being used. The ApiToken property holds the authentication token required for API requests, while the AccountId property specifies the unique identifier of the Cloudflare account. The QueueIdOrName property identifies the specific queue to be accessed within the account.
/// </summary>
public record CloudFlareAccountDetails : IAccountDetails
{
    /// <summary>
    /// Gets the version of the Cloudflare API being used. This property is initialized to "v4" by default, indicating that the application is interacting with version 4 of the Cloudflare API. The API version is important for ensuring compatibility with the API endpoints and features available in that specific version.
    /// </summary>
    public string? ApiVersion { get; init; } = "v4";
    /// <summary>
    /// Gets the API token used for authenticating requests to the Cloudflare API. This token is required for accessing the account and performing operations on its queues. The ApiToken property should be securely stored and managed, as it provides access to sensitive account information and operations.
    /// </summary>
    public string? ApiToken { get; init; }
    /// <summary>
    /// Gets the unique identifier of the Cloudflare account. This property is used to specify which account the application is interacting with when making API requests. The AccountId is essential for ensuring that operations are performed on the correct account and its associated resources, such as queues.
    /// </summary>
    public string? AccountId { get; init; }
    /// <summary>
    /// Gets the identifier or name of the specific queue within the Cloudflare account that the application will interact with. This property is used to specify which queue to access for operations such as pulling messages, adding messages, or managing queue settings. The QueueIdOrName property allows for flexibility in identifying the queue by either its unique ID or its human-readable name.
    /// </summary>
    public string? QueueIdOrName { get; init; }

}
