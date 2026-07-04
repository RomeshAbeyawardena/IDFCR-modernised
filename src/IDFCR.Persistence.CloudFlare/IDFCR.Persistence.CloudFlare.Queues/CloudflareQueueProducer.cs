using IDFCR.Abstractions.Persistence.StorageQueues;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace IDFCR.Persistence.CloudFlare.Queues;

public abstract class CloudflareClient(
    IAccountDetails accountDetails,
    HttpClient httpClient)
{
    private const string BaseUrl = "https://api.cloudflare.com/client/";

    private readonly Lazy<AuthenticationHeaderValue> authenticationHeader
        = new(() => new AuthenticationHeaderValue("Bearer",
            accountDetails.ApiToken));

    protected IAccountDetails AccountDetails { get; } = accountDetails;

    protected string AuthenticatedBaseUrl { get; } = $"{BaseUrl}/{accountDetails.ApiVersion}/accounts/{accountDetails.AccountId}";

    protected HttpClient HttpClient { get; }

    protected void PrepareClient()
    {
        HttpClient.DefaultRequestHeaders.Authorization
            = authenticationHeader.Value;
    }
}

/// <summary>
/// Represents a producer that can send messages to a Cloudflare queue. The SendMessageAsync method takes a payload of type T and sends it to the specified Cloudflare queue asynchronously, returning a boolean indicating whether the message was sent successfully. This class implements the IQueueProducer interface and uses an HttpClient to send HTTP requests to the Cloudflare API. The class requires account details, including an API token, account ID, queue ID or name, and API version, which are provided through the IAccountDetails interface.
/// </summary>
/// <param name="accountDetails">The account details required to authenticate and interact with the Cloudflare API.</param>
/// <param name="httpClient">The HttpClient used to send HTTP requests to the Cloudflare API.</param>
public class CloudflareQueueProducer(
    IAccountDetails accountDetails,
    HttpClient httpClient) : CloudflareClient(accountDetails, httpClient), IQueueProducer
{
    /// <summary>
    /// Sends a message with the specified payload to the Cloudflare queue asynchronously. The method constructs the appropriate URL for the Cloudflare API based on the provided account details, serializes the payload into JSON format, and sends it as an HTTP POST request. The method returns a boolean indicating whether the message was sent successfully, based on the HTTP response status code.
    /// </summary>
    /// <typeparam name="T">The type of the message payload.</typeparam>
    /// <param name="payload">The message payload to send.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a boolean indicating whether the message was sent successfully.</returns>
    public async Task<bool> SendMessageAsync<T>(T payload)
    {
        var url = $"{AuthenticatedBaseUrl}/queues/{AccountDetails.QueueIdOrName}/messages";

        var envelope = new { body = payload };
        var jsonContent = new StringContent(JsonSerializer.Serialize(envelope), System.Text.Encoding.UTF8, "application/json");

        var response = await HttpClient.PostAsync(url, jsonContent);
        return response.IsSuccessStatusCode;
    }
}

public class CloudFlareQueueConsumer(
    IAccountDetails accountDetails,
    HttpClient httpClient) 
    : CloudflareClient(accountDetails, httpClient)
    , IQueueConsumer<CloudflarePullResponse, CloudflareQueuePullResult, CloudflareQueueMessageItem,
        CloudflareApiError, JsonElement>
{
    public async Task AcknowledgeMessageAsync(string messageId, CancellationToken cancellationToken)
    {
        
    }

    public async Task<IEnumerable<CloudflareQueueMessageItem>> PullMessagesAsync(
        int VisibilityTimeout, 
        int BatchSize, 
        CancellationToken cancellationToken)
    {
        List<CloudflareQueueMessageItem> messages = [];
        var url = $"{AuthenticatedBaseUrl}/queues/{AccountDetails.QueueIdOrName}/messages/pull";

        var requestBody = new { visibility_timeout = VisibilityTimeout, batch_size = BatchSize };
        var response = await HttpClient.PostAsJsonAsync(url, requestBody, cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<CloudflarePullResponse>(cancellationToken: cancellationToken);

            if (result?.Result?.Messages != null && result.Result.Messages.Count != 0)
            {
                messages.AddRange(result.Result.Messages);
            }
        }

        return messages;
    }
}
