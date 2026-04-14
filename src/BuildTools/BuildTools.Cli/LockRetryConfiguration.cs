namespace BuildTools.Cli;

public record LockRetryConfiguration(int? MaximumAttempts, int? RetryTimeoutInMilliseconds, int? LockTimeoutInMinutes)
{
    public LockRetryConfiguration() : this(null, null, null)
    {

    }
}

public record ApplicationConfiguration(bool ListOperations)
{
    public ApplicationConfiguration() : this(false)
    {
        
    }
}