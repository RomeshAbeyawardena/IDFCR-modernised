using IDFCR.AI.Abstractions;
using IDFCR.AI.Http.Configurations;
using System.Net.Http.Headers;

namespace IDFCR.AI.Http;

/// <summary>
/// Default <see cref="IAIService"/> implementation that sends requests with <see cref="HttpClient"/>.
/// </summary>
public sealed class HttpAIService(HttpClient httpClient) : IAIService
{
    /// <inheritdoc />
    public async Task<VerifiedConnectionResult> VerifyConnection<TConfiguration>(TConfiguration configuration, CancellationToken cancellationToken)
        where TConfiguration : IAIServiceConfiguration
    {
        var httpConfiguration = GetHttpConfiguration(configuration);
        var request = new AIServiceRequest
        {
            Method = httpConfiguration.VerificationMethod,
            RelativePath = httpConfiguration.VerificationPath,
            Content = httpConfiguration.VerificationContent,
            ContentType = httpConfiguration.VerificationContentType
        };

        try
        {
            var response = await SendAsync(httpConfiguration, request, cancellationToken);
            return response.IsSuccessStatusCode
                ? VerifiedConnectionResult.Success(response.StatusCode)
                : VerifiedConnectionResult.Failure("The AI service responded, but the connection was not accepted.", response.StatusCode);
        }
        catch (Exception exception)
        {
            return VerifiedConnectionResult.Failure(exception.Message);
        }
    }

    /// <inheritdoc />
    public async Task<AIServiceResponse> SendAsync<TConfiguration>(TConfiguration configuration, AIServiceRequest request, CancellationToken cancellationToken)
        where TConfiguration : IAIServiceConfiguration
    {
        ArgumentNullException.ThrowIfNull(request);

        var httpConfiguration = GetHttpConfiguration(configuration);
        using var message = CreateRequestMessage(httpConfiguration, request);
        using var linkedCancellationTokenSource = CreateCancellationTokenSource(httpConfiguration.Timeout, cancellationToken);
        using var response = await httpClient.SendAsync(message, linkedCancellationTokenSource.Token);
        var content = response.Content is null
            ? null
            : await response.Content.ReadAsStringAsync(linkedCancellationTokenSource.Token);

        return new AIServiceResponse(response.StatusCode, content, ReadHeaders(response));
    }

    private static HttpAIServiceConfiguration GetHttpConfiguration<TConfiguration>(TConfiguration configuration)
        where TConfiguration : IAIServiceConfiguration
    {
        ArgumentNullException.ThrowIfNull(configuration);

        return configuration as HttpAIServiceConfiguration
            ?? throw new NotSupportedException($"{typeof(HttpAIService).Name} requires a {nameof(HttpAIServiceConfiguration)}.");
    }

    private static HttpRequestMessage CreateRequestMessage(HttpAIServiceConfiguration configuration, AIServiceRequest request)
    {
        var relativePath = string.IsNullOrWhiteSpace(request.RelativePath) ? "/" : request.RelativePath;
        var requestUri = new Uri(configuration.BaseAddress, relativePath);
        var message = new HttpRequestMessage(new HttpMethod(request.Method), requestUri);

        ApplyHeaders(message.Headers, configuration.DefaultHeaders);
        ApplyHeaders(message.Headers, request.Headers);

        if (request.Content is not null)
        {
            message.Content = new StringContent(request.Content);

            if (!string.IsNullOrWhiteSpace(request.ContentType))
            {
                message.Content.Headers.ContentType = MediaTypeHeaderValue.Parse(request.ContentType);
            }
        }

        return message;
    }

    private static CancellationTokenSource CreateCancellationTokenSource(TimeSpan? timeout, CancellationToken cancellationToken)
    {
        var source = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        if (timeout is { } configuredTimeout)
        {
            source.CancelAfter(configuredTimeout);
        }

        return source;
    }

    private static void ApplyHeaders(HttpRequestHeaders headers, IEnumerable<KeyValuePair<string, string>> values)
    {
        foreach (var (key, value) in values)
        {
            headers.Remove(key);
            headers.TryAddWithoutValidation(key, value);
        }
    }

    private static IReadOnlyDictionary<string, IReadOnlyCollection<string>> ReadHeaders(HttpResponseMessage response)
    {
        var headers = response.Headers.AsEnumerable();

        if (response.Content is not null)
        {
            headers = headers.Concat(response.Content.Headers);
        }

        return headers
            .ToDictionary(
                pair => pair.Key,
                pair => (IReadOnlyCollection<string>)pair.Value.ToArray(),
                StringComparer.OrdinalIgnoreCase);
    }
}
