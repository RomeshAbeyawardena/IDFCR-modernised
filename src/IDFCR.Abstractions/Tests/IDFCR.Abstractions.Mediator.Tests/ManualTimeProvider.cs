namespace IDFCR.Abstractions.Mediator.Tests;

// ── Minimal TimeProvider stub ─────────────────────────────────────────────────

public sealed class ManualTimeProvider(DateTimeOffset utcNow) : TimeProvider
{
    public override DateTimeOffset GetUtcNow() => utcNow;
}
