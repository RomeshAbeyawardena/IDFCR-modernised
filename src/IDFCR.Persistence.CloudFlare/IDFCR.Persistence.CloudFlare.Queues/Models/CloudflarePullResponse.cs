using IDFCR.Abstractions.Persistence.StorageQueues;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace IDFCR.Persistence.CloudFlare.Queues.Models;

/// <summary>
/// Represents the response received after pulling messages from a Cloudflare queue. The response includes information about the success of the operation, any errors that occurred, messages related to the operation, and the result containing the pulled messages. This class is used to deserialize the JSON response from the Cloudflare API when interacting with queues.
/// </summary>
public class CloudFlarePullResponse
    : IQueuePullResponse<CloudFlareQueuePullResult, CloudFlareQueueMessageItem, CloudFlareApiError, JsonElement>
{
    /// <summary>
    /// Gets or sets a value indicating whether the pull operation was successful. A value of true indicates that the operation was successful, while false indicates that there were errors during the operation.
    /// </summary>
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    /// <summary>
    /// Gets or sets a list of errors that occurred during the pull operation. Each error is represented by an instance of the CloudflareApiError class, which provides details about the specific error, including an error code and message. This property allows for identification and handling of any issues that arose during the operation.
    /// </summary>
    [JsonPropertyName("errors")]
    public List<CloudFlareApiError> Errors { get; set; } = [];

    /// <summary>
    /// Gets or sets a list of messages related to the pull operation. These messages may provide additional context or information about the operation, such as warnings or informational messages. This property allows for better understanding of the operation's outcome and any relevant details that may be useful for debugging or logging purposes.
    /// </summary>
    [JsonPropertyName("messages")]
    public List<string> Messages { get; set; } = [];

    /// <summary>
    /// Gets or sets the result of the pull operation, which contains the pulled messages and any additional information related to the operation. The result is represented by an instance of the QueuePullResult class, which provides details about the messages that were successfully pulled from the queue. This property allows for access to the actual data retrieved from the queue as a result of the pull operation.
    /// </summary>
    [JsonPropertyName("result")]
    public CloudFlareQueuePullResult? Result { get; set; }
    IEnumerable<CloudFlareApiError> IQueuePullResponse<CloudFlareQueuePullResult, CloudFlareQueueMessageItem, CloudFlareApiError, JsonElement>.Errors => Errors;
    IEnumerable<string> IQueuePullResponse<CloudFlareQueuePullResult, CloudFlareQueueMessageItem, CloudFlareApiError, JsonElement>.Messages => Messages;
}
