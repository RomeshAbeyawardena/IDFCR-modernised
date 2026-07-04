using IDFCR.Persistence.CloudFlare.Queues.Models;
using System.Net.Http.Headers;

namespace IDFCR.Persistence.CloudFlare.Queues;

/// <summary>
/// Represents a base class for interacting with the Cloudflare API. This abstract class provides common functionality for making authenticated HTTP requests to the Cloudflare API, including preparing the HttpClient with the necessary authentication headers. It requires account details, including an API token, account ID, and API version, which are provided through the IAccountDetails interface. Derived classes can implement specific functionality for interacting with different aspects of the Cloudflare API, such as sending messages to queues or managing queue settings.
/// </summary>
/// <param name="accountDetails">The account details required to authenticate and interact with the Cloudflare API.</param>
/// <param name="httpClient">The HttpClient used to send HTTP requests to the Cloudflare API.</param>
public abstract class CloudflareClient(
    IAccountDetails accountDetails,
    HttpClient httpClient)
{
    private const string BaseUrl = "https://api.cloudflare.com/client/";

    private readonly Lazy<AuthenticationHeaderValue> authenticationHeader
        = new(() => new AuthenticationHeaderValue("Bearer",
            accountDetails.ApiToken));

    /// <summary>
    /// Gets the account details required to authenticate and interact with the Cloudflare API. This property provides access to the API token, account ID, queue ID or name, and API version, which are necessary for making authenticated requests to the Cloudflare API. Implementations of this class can use the AccountDetails property to retrieve the necessary information for constructing API requests and interacting with Cloudflare queues.
    /// </summary>
    protected IAccountDetails AccountDetails { get; } = accountDetails;

    /// <summary>
    /// Gets the base URL for authenticated requests to the Cloudflare API. This property constructs the URL based on the provided account details, including the API version and account ID. The AuthenticatedBaseUrl property is used by derived classes to construct specific API endpoints for interacting with Cloudflare queues and other resources. It ensures that requests are made to the correct account and API version, allowing for proper authentication and access to the desired resources.
    /// </summary>
    protected string AuthenticatedBaseUrl { get; } = $"{BaseUrl}/{accountDetails.ApiVersion}/accounts/{accountDetails.AccountId}";

    /// <summary>
    /// Gets the HttpClient used to send HTTP requests to the Cloudflare API. This property provides access to the HttpClient instance that is configured with the necessary authentication headers for making requests to the Cloudflare API. Derived classes can use the HttpClient property to send requests to specific API endpoints, such as sending messages to queues or managing queue settings. The HttpClient is prepared with the appropriate authentication headers by calling the PrepareClient method, ensuring that requests are properly authenticated and authorized.
    /// </summary>
    protected HttpClient HttpClient { get; } = httpClient;

    /// <summary>
    /// Gets the authentication header value used for making authenticated requests to the Cloudflare API. This property is lazily initialized and provides the necessary authentication information, including the API token, for constructing the Authorization header in HTTP requests. The AuthenticationHeader property is used by derived classes to prepare the HttpClient with the appropriate authentication headers before sending requests to the Cloudflare API.
    /// </summary>
    protected void PrepareClient()
    {
        HttpClient.DefaultRequestHeaders.Authorization
            = authenticationHeader.Value;
    }
}
