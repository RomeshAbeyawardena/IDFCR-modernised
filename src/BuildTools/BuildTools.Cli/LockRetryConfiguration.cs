using System;
using System.Collections.Generic;
using System.Text;

namespace BuildTools.Cli;

public record LockRetryConfiguration(int? MaximumAttempts, int? RetryTimeoutInMilliseconds, int? LockTimeoutInMinutes)
{
    
}
