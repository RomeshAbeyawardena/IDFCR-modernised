using IDFCR.Abstractions.Persistence.StorageQueues;
using IDFCR.Abstractions.Results;
using IDFCR.Persistence.CloudFlare.Queues.Models;
using System.Net.Http.Json;
using System.Text.Json;

namespace IDFCR.Persistence.CloudFlare.Queues;

/// <summary>
/// Represents a consumer that can pull and acknowledge messages from a Cloudflare queue. The class implements the IQueueConsumer interface, allowing for pulling messages with specified visibility timeout and batch size, as well as acknowledging messages by their ID. The class uses an HttpClient to send HTTP requests to the Cloudflare API, and requires account details provided through the IAccountDetails interface for authentication and queue identification.
/// </summary>
/// <param name="accountDetails">The account details required to authenticate and interact with the Cloudflare API.</param>
/// <param name="httpClient">The HttpClient used to send HTTP requests to the Cloudflare API.</param>
public class CloudFlareQueueConsumer(
    IAccountDetails accountDetails,
    HttpClient httpClient) 
    : CloudFlareClient(accountDetails, httpClient)
    , IQueueConsumer<CloudFlarePullResponse, CloudFlareQueuePullResult, CloudFlareQueueMessageItem,
        CloudFlareApiError, JsonElement>
{
    /// <summary>
    /// Acknowledges a message with the specified message ID from the Cloudflare queue asynchronously. The method constructs the appropriate URL for the Cloudflare API based on the provided account details and sends an HTTP POST request to acknowledge the message. The method does not return a value, but it ensures that the message is acknowledged and removed from the queue, preventing it from being processed multiple times.
    /// </summary>
    /// <param name="messageId">The ID of the message to acknowledge.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task<IUnitResult> AcknowledgeMessageAsync(string messageId, CancellationToken cancellationToken)
    {
        PrepareClientOnce();
        var url = $"{ServiceDefinitions.QueueRelativeUrl}/messages/ack";

        var ackPayload = new CloudFlareAckRequest
        {
            Acks = [new() { LeaseId = messageId }]
        };

        try
        {
            // 3. Issue the POST request to the acknowledgment endpoint
            var response = await HttpClient.PostAsJsonAsync(url, ackPayload, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);

                return UnitResult.Failed(new Exception($"Failed to ACK message lease {messageId}. Status: {response.StatusCode}. Details: {errorContent}"), UnitAction.None, FailureReason.ExternalDependencyError);
            }

            return UnitResult.Success(UnitAction.Delete);
        }
        catch (Exception ex)
        {
            return UnitResult.Failed(ex, UnitAction.None, FailureReason.ExternalDependencyError);
        }

    }

    /// <summary>
    /// Pulls messages from the Cloudflare queue asynchronously with the specified visibility timeout and batch size. The method constructs the appropriate URL for the Cloudflare API based on the provided account details, sends an HTTP POST request to pull messages, and returns a collection of pulled messages. The visibility timeout determines how long the messages will be hidden from other consumers after being pulled, and the batch size specifies how many messages to pull in a single request.
    /// </summary>
    /// <param name="visibilityTimeout">The visibility timeout for the pulled messages.</param>
    /// <param name="batchSize">The number of messages to pull in a single request.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a collection of pulled messages.</returns>
    public async Task<IUnitResultCollection<CloudFlareQueueMessageItem>> PullMessagesAsync(
        int visibilityTimeout, 
        int batchSize, 
        CancellationToken cancellationToken)
    {
        PrepareClientOnce();
        try
        {
            List<CloudFlareQueueMessageItem> messages = [];
            var url = $"{ServiceDefinitions.QueueRelativeUrl}/messages/pull";

            var requestBody = new
            {
                visibility_timeout_ms = visibilityTimeout,
                batch_size = batchSize
            };

            var json = JsonSerializer.Serialize(requestBody);

            using var content = new StringContent(
                json, System.Text.Encoding.UTF8, "application/json");

            content.Headers.ContentLength = System.Text.Encoding.UTF8.GetByteCount(json);

            var response = await HttpClient.PostAsync(url, content, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<CloudFlarePullResponse>(cancellationToken: cancellationToken);

                if (result?.Result?.Messages != null && result.Result.Messages.Count != 0)
                {
                    messages.AddRange(result.Result.Messages);
                }
            }

            return UnitResultCollection.Failed<CloudFlareQueueMessageItem>(response);
        }
        catch (Exception ex)
        {
            return UnitResultCollection.Failed<CloudFlareQueueMessageItem>(ex);
        }
    }
}
