using IDFCR.Persistence.CloudFlare.Queues.Models;

namespace IDFCR.Persistence.CloudFlare.Queues;

/// <summary>
/// Represents the service definitions for interacting with the Cloudflare API. This class provides properties for constructing the authenticated base URL and queue relative URL based on the provided account details, including the API version, account ID, and queue ID or name. The ServiceDefinitions class is used by other components to build specific API endpoints for interacting with Cloudflare queues and other resources, ensuring that requests are made to the correct account and API version.
/// </summary>
public class ServiceDefinitions(IAccountDetails accountDetails)
{
    /// <summary>
    /// Builds a new instance of the ServiceDefinitions class using the provided account details. This static method allows for convenient creation of a ServiceDefinitions instance, encapsulating the necessary information for constructing API endpoints for interacting with Cloudflare queues and other resources. The Build method ensures that the ServiceDefinitions instance is properly initialized with the required account details, including the API version, account ID, and queue ID or name.
    /// </summary>
    /// <param name="accountDetails">The account details required to construct the service definitions.</param>
    /// <returns>A new instance of the ServiceDefinitions class.</returns>
    public static ServiceDefinitions Build(IAccountDetails accountDetails) => new(accountDetails);

    private const string BaseUrl = "https://api.cloudflare.com/client";

    /// <summary>
    /// Gets the authenticated base URL for making requests to the Cloudflare API. This property constructs the URL based on the provided account details, including the API version and account ID. The AuthenticatedBaseUrl property is used by other components to build specific API endpoints for interacting with Cloudflare queues and other resources, ensuring that requests are made to the correct account and API version.
    /// </summary>
    public string AuthenticatedBaseUrl { get; } = $"{BaseUrl}/{accountDetails.ApiVersion}/accounts/{accountDetails.AccountId}/";

    /// <summary>
    /// Gets the queue relative URL for making requests to the Cloudflare API. This property constructs the URL based on the provided account details, including the queue ID or name. The QueueRelativeUrl property is used by other components to build specific API endpoints for interacting with Cloudflare queues, ensuring that requests are made to the correct queue within the specified account.
    /// </summary>
    public string QueueRelativeUrl { get; } = $"queues/{accountDetails.QueueIdOrName}";
}
