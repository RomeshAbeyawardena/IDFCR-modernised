namespace IDFCR.Abstractions.Caching;

/// <summary>
/// Represents a service that provides caching functionality for string values. It allows you to store and retrieve a cached string value in a thread-safe manner. The service uses a semaphore to control access to the cached value, ensuring that only one thread can access it at a time. The initial and maximum count for the semaphore can be configured through the constructor parameters. This service is useful for scenarios where you want to cache a string value and ensure that it is accessed in a thread-safe way.
/// </summary>
/// <param name="initialCount">The initial number of requests.</param>
/// <param name="maxCount">The maximum number of requests.</param>
public sealed class CachedStringService(int? initialCount = null, int? maxCount = null) : IDisposable
{
    private readonly SemaphoreSlim semaphoreSlim = new(initialCount.GetValueOrDefault(1), maxCount.GetValueOrDefault(1));
    private string? _value;
    /// <summary>
    /// Gets the cached string value in a thread-safe manner. It waits for the semaphore to be available before accessing the cached value, ensuring that only one thread can access it at a time. If the cached value is not set, it returns null. The method accepts an optional cancellation token that can be used to cancel the operation if needed.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The cached string value, or null if not set.</returns>
    public async Task<string?> GetCachedValueAsync(CancellationToken? cancellationToken = null)
    {
        try
        {
            await semaphoreSlim.WaitAsync(cancellationToken.GetValueOrDefault(CancellationToken.None));
            return _value;
        }
        finally
        {
            semaphoreSlim.Release(1);
        }
    }

    /// <summary>
    /// Sets the cached string value in a thread-safe manner. It waits for the semaphore to be available before setting the cached value, ensuring that only one thread can access it at a time. The method accepts the new value to be cached and an optional cancellation token that can be used to cancel the operation if needed. It returns the old cached value before it was updated, or null if there was no previous value.
    /// </summary>
    /// <param name="value">The new value to be cached.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The old cached value, or null if there was no previous value.</returns>
    public async Task<string?> SetCachedValueAsync(string value, CancellationToken? cancellationToken = null)
    {
        try
        {
            await semaphoreSlim.WaitAsync(cancellationToken.GetValueOrDefault(CancellationToken.None));
            var oldValue = value;
            _value = value;

            return oldValue;
        }
        finally
        {
            semaphoreSlim.Release(1);
        }
    }

    /// <summary>
    /// Disposes of the resources used by the CachedStringService. It suppresses finalization and disposes of the semaphore to release any resources it holds. This method should be called when the CachedStringService is no longer needed to ensure that resources are properly released.
    /// </summary>
    public void Dispose()
    {
        GC.SuppressFinalize(this);
        semaphoreSlim?.Dispose();
    }
}