using IDFCR.Abstractions.Persistence.StorageQueues;
using IDFCR.Abstractions.Results;
using IDFCR.Persistence.CloudFlare.Queues.Models;
using System.Text.Json;

namespace IDFCR.Persistence.CloudFlare.Queues;

/// <summary>
/// Represents a producer that can send messages to a Cloudflare queue. The SendMessageAsync method takes a payload of type T and sends it to the specified Cloudflare queue asynchronously, returning a boolean indicating whether the message was sent successfully. This class implements the IQueueProducer interface and uses an HttpClient to send HTTP requests to the Cloudflare API. The class requires account details, including an API token, account ID, queue ID or name, and API version, which are provided through the IAccountDetails interface.
/// </summary>
/// <param name="accountDetails">The account details required to authenticate and interact with the Cloudflare API.</param>
/// <param name="httpClient">The HttpClient used to send HTTP requests to the Cloudflare API.</param>
public class CloudFlareQueueProducer(
    IAccountDetails accountDetails,
    HttpClient httpClient) : CloudFlareClient(accountDetails, httpClient), IQueueProducer
{
    /// <summary>
    /// Sends a message with the specified payload to the Cloudflare queue asynchronously. The method constructs the appropriate URL for the Cloudflare API based on the provided account details, serializes the payload into JSON format, and sends it as an HTTP POST request. The method returns a boolean indicating whether the message was sent successfully, based on the HTTP response status code.
    /// </summary>
    /// <typeparam name="T">The type of the message payload.</typeparam>
    /// <param name="payload">The message payload to send.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a boolean indicating whether the message was sent successfully.</returns>
    public async Task<IUnitResult> SendMessageAsync<T>(T payload)
    {
        PrepareClientOnce();
        var url = $"{ServiceDefinitions.QueueRelativeUrl}/messages";

        var envelope = new { body = payload };
        var jsonContent = new StringContent(JsonSerializer.Serialize(envelope), System.Text.Encoding.UTF8, "application/json");

        var response = await HttpClient.PostAsync(url, jsonContent);
        if (response.IsSuccessStatusCode)
        {
            return UnitResult.Success(UnitAction.Add);
        }

        var errorContent = await response.Content.ReadAsStringAsync();
        return UnitResult.Failed(new Exception(errorContent), UnitAction.None, FailureReason.ExternalDependencyError);
    }
}