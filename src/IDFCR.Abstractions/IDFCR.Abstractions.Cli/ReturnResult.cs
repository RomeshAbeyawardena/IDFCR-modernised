namespace IDFCR.Abstractions.Cli;

/// <summary>
/// Provides thread-safe access to a static integer result value.
/// </summary>
public static class ReturnResult
{
    private static int? _returnResult;
    private static readonly Lock _lock = new();
    /// <summary>
    /// Gets or sets the return result value in a thread-safe manner.
    /// </summary>
    public static int? Value 
    {
        get {

            lock (_lock)
            {
                return _returnResult;
            }
        }
        set {
            lock (_lock)
            {
                _returnResult = value;
            }
        }
    }
}
