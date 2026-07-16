using IDFCR.Abstractions.Filters;

namespace IDFCR.Abstractions.Filters.Tests.Assets;

public sealed class FirstObjectCustomerLookup : IAsyncLookup<Customer>, IDisposable
{
    public Task<bool> CanLookupAsync(object? filter, CancellationToken cancellationToken)
    {
        AsyncLookupTestLog.Record(nameof(FirstObjectCustomerLookup), nameof(CanLookupAsync), filter, cancellationToken);
        return Task.FromResult(filter is AsyncLookupObjectFilter);
    }

    public Task<Customer> LookupAsync(object? filter, CancellationToken cancellationToken)
    {
        AsyncLookupTestLog.Record(nameof(FirstObjectCustomerLookup), nameof(LookupAsync), filter, cancellationToken);
        return Task.FromResult(new Customer
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
            FirstName = "First",
            LastName = "Object",
            CreatedTimestampUtc = DateTimeOffset.UtcNow
        });
    }

    public void Dispose()
    {
        AsyncLookupTestLog.Record(nameof(FirstObjectCustomerLookup), nameof(Dispose), null, default);
    }
}

public sealed class SecondObjectCustomerLookup : IAsyncLookup<Customer>, IDisposable
{
    public Task<bool> CanLookupAsync(object? filter, CancellationToken cancellationToken)
    {
        AsyncLookupTestLog.Record(nameof(SecondObjectCustomerLookup), nameof(CanLookupAsync), filter, cancellationToken);
        return Task.FromResult(filter is AsyncLookupObjectFilter);
    }

    public Task<Customer> LookupAsync(object? filter, CancellationToken cancellationToken)
    {
        AsyncLookupTestLog.Record(nameof(SecondObjectCustomerLookup), nameof(LookupAsync), filter, cancellationToken);
        return Task.FromResult(new Customer
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000002"),
            FirstName = "Second",
            LastName = "Object",
            CreatedTimestampUtc = DateTimeOffset.UtcNow
        });
    }

    public void Dispose()
    {
        AsyncLookupTestLog.Record(nameof(SecondObjectCustomerLookup), nameof(Dispose), null, default);
    }
}

public sealed class SkippingObjectCustomerLookup : IAsyncLookup<Customer>, IDisposable
{
    public Task<bool> CanLookupAsync(object? filter, CancellationToken cancellationToken)
    {
        AsyncLookupTestLog.Record(nameof(SkippingObjectCustomerLookup), nameof(CanLookupAsync), filter, cancellationToken);
        return Task.FromResult(false);
    }

    public Task<Customer> LookupAsync(object? filter, CancellationToken cancellationToken)
    {
        AsyncLookupTestLog.Record(nameof(SkippingObjectCustomerLookup), nameof(LookupAsync), filter, cancellationToken);
        throw new InvalidOperationException("LookupAsync should not be called when CanLookupAsync returns false.");
    }

    public void Dispose()
    {
        AsyncLookupTestLog.Record(nameof(SkippingObjectCustomerLookup), nameof(Dispose), null, default);
    }
}

public sealed class ThrowingObjectCustomerLookup : IAsyncLookup<Customer>, IDisposable
{
    public Task<bool> CanLookupAsync(object? filter, CancellationToken cancellationToken)
    {
        AsyncLookupTestLog.Record(nameof(ThrowingObjectCustomerLookup), nameof(CanLookupAsync), filter, cancellationToken);
        return Task.FromResult(filter is AsyncLookupObjectFilter);
    }

    public Task<Customer> LookupAsync(object? filter, CancellationToken cancellationToken)
    {
        AsyncLookupTestLog.Record(nameof(ThrowingObjectCustomerLookup), nameof(LookupAsync), filter, cancellationToken);
        throw new InvalidOperationException("Object lookup failure.");
    }

    public void Dispose()
    {
        AsyncLookupTestLog.Record(nameof(ThrowingObjectCustomerLookup), nameof(Dispose), null, default);
    }
}

public sealed class FirstTypedCustomerLookup : AsyncLookupBase<Customer, AsyncLookupTypedFilter>, IDisposable
{
    public override Task<bool> CanLookupAsync(object? filter, CancellationToken cancellationToken)
    {
        AsyncLookupTestLog.Record(nameof(FirstTypedCustomerLookup), nameof(CanLookupAsync), filter, cancellationToken);
        return base.CanLookupAsync(filter, cancellationToken);
    }

    public override Task<Customer> LookupAsync(AsyncLookupTypedFilter filter, CancellationToken cancellationToken)
    {
        AsyncLookupTestLog.Record(nameof(FirstTypedCustomerLookup), nameof(LookupAsync), filter, cancellationToken);
        return Task.FromResult(new Customer
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000101"),
            FirstName = "First",
            LastName = "Typed",
            CreatedTimestampUtc = DateTimeOffset.UtcNow
        });
    }

    public void Dispose()
    {
        AsyncLookupTestLog.Record(nameof(FirstTypedCustomerLookup), nameof(Dispose), null, default);
    }
}

public sealed class SecondTypedCustomerLookup : AsyncLookupBase<Customer, AsyncLookupTypedFilter>, IDisposable
{
    public override Task<bool> CanLookupAsync(object? filter, CancellationToken cancellationToken)
    {
        AsyncLookupTestLog.Record(nameof(SecondTypedCustomerLookup), nameof(CanLookupAsync), filter, cancellationToken);
        return base.CanLookupAsync(filter, cancellationToken);
    }

    public override Task<Customer> LookupAsync(AsyncLookupTypedFilter filter, CancellationToken cancellationToken)
    {
        AsyncLookupTestLog.Record(nameof(SecondTypedCustomerLookup), nameof(LookupAsync), filter, cancellationToken);
        return Task.FromResult(new Customer
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000102"),
            FirstName = "Second",
            LastName = "Typed",
            CreatedTimestampUtc = DateTimeOffset.UtcNow
        });
    }

    public void Dispose()
    {
        AsyncLookupTestLog.Record(nameof(SecondTypedCustomerLookup), nameof(Dispose), null, default);
    }
}

public sealed class SkippingTypedCustomerLookup : AsyncLookupBase<Customer, AsyncLookupTypedFilter>, IDisposable
{
    public override Task<bool> CanLookupAsync(object? filter, CancellationToken cancellationToken)
    {
        AsyncLookupTestLog.Record(nameof(SkippingTypedCustomerLookup), nameof(CanLookupAsync), filter, cancellationToken);
        return Task.FromResult(false);
    }

    public override Task<Customer> LookupAsync(AsyncLookupTypedFilter filter, CancellationToken cancellationToken)
    {
        AsyncLookupTestLog.Record(nameof(SkippingTypedCustomerLookup), nameof(LookupAsync), filter, cancellationToken);
        throw new InvalidOperationException("LookupAsync should not be called when CanLookupAsync returns false.");
    }

    public void Dispose()
    {
        AsyncLookupTestLog.Record(nameof(SkippingTypedCustomerLookup), nameof(Dispose), null, default);
    }
}

public sealed class ThrowingTypedCustomerLookup : AsyncLookupBase<Customer, AsyncLookupTypedFilter>, IDisposable
{
    public override Task<bool> CanLookupAsync(object? filter, CancellationToken cancellationToken)
    {
        AsyncLookupTestLog.Record(nameof(ThrowingTypedCustomerLookup), nameof(CanLookupAsync), filter, cancellationToken);
        return base.CanLookupAsync(filter, cancellationToken);
    }

    public override Task<Customer> LookupAsync(AsyncLookupTypedFilter filter, CancellationToken cancellationToken)
    {
        AsyncLookupTestLog.Record(nameof(ThrowingTypedCustomerLookup), nameof(LookupAsync), filter, cancellationToken);
        throw new InvalidOperationException("Typed lookup failure.");
    }

    public void Dispose()
    {
        AsyncLookupTestLog.Record(nameof(ThrowingTypedCustomerLookup), nameof(Dispose), null, default);
    }
}

public sealed class ThrowingCanLookupObjectCustomerLookup : IAsyncLookup<Customer>, IDisposable
{
    public Task<bool> CanLookupAsync(object? filter, CancellationToken cancellationToken)
    {
        AsyncLookupTestLog.Record(nameof(ThrowingCanLookupObjectCustomerLookup), nameof(CanLookupAsync), filter, cancellationToken);
        throw new InvalidOperationException("Object can-lookup failure.");
    }

    public Task<Customer> LookupAsync(object? filter, CancellationToken cancellationToken)
    {
        AsyncLookupTestLog.Record(nameof(ThrowingCanLookupObjectCustomerLookup), nameof(LookupAsync), filter, cancellationToken);
        throw new InvalidOperationException("LookupAsync should not be called after CanLookupAsync throws.");
    }

    public void Dispose()
    {
        AsyncLookupTestLog.Record(nameof(ThrowingCanLookupObjectCustomerLookup), nameof(Dispose), null, default);
    }
}

public sealed class ThrowingCanLookupTypedCustomerLookup : AsyncLookupBase<Customer, AsyncLookupTypedFilter>, IDisposable
{
    public override Task<bool> CanLookupAsync(object? filter, CancellationToken cancellationToken)
    {
        AsyncLookupTestLog.Record(nameof(ThrowingCanLookupTypedCustomerLookup), nameof(CanLookupAsync), filter, cancellationToken);
        throw new InvalidOperationException("Typed can-lookup failure.");
    }

    public override Task<Customer> LookupAsync(AsyncLookupTypedFilter filter, CancellationToken cancellationToken)
    {
        AsyncLookupTestLog.Record(nameof(ThrowingCanLookupTypedCustomerLookup), nameof(LookupAsync), filter, cancellationToken);
        throw new InvalidOperationException("LookupAsync should not be called after CanLookupAsync throws.");
    }

    public void Dispose()
    {
        AsyncLookupTestLog.Record(nameof(ThrowingCanLookupTypedCustomerLookup), nameof(Dispose), null, default);
    }
}

public sealed class InterfaceFilterTypedCustomerLookup : AsyncLookupBase<Customer, IAsyncLookupInterfaceFilter>, IDisposable
{
    public override Task<bool> CanLookupAsync(object? filter, CancellationToken cancellationToken)
    {
        AsyncLookupTestLog.Record(nameof(InterfaceFilterTypedCustomerLookup), nameof(CanLookupAsync), filter, cancellationToken);
        return base.CanLookupAsync(filter, cancellationToken);
    }

    public override Task<Customer> LookupAsync(IAsyncLookupInterfaceFilter filter, CancellationToken cancellationToken)
    {
        AsyncLookupTestLog.Record(nameof(InterfaceFilterTypedCustomerLookup), nameof(LookupAsync), filter, cancellationToken);
        return Task.FromResult(new Customer
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000201"),
            FirstName = "Interface",
            LastName = "Typed",
            CreatedTimestampUtc = DateTimeOffset.UtcNow
        });
    }

    public void Dispose()
    {
        AsyncLookupTestLog.Record(nameof(InterfaceFilterTypedCustomerLookup), nameof(Dispose), null, default);
    }
}

public sealed class SkippingInterfaceFilterTypedCustomerLookup : AsyncLookupBase<Customer, IAsyncLookupInterfaceFilter>, IDisposable
{
    public override Task<bool> CanLookupAsync(object? filter, CancellationToken cancellationToken)
    {
        AsyncLookupTestLog.Record(nameof(SkippingInterfaceFilterTypedCustomerLookup), nameof(CanLookupAsync), filter, cancellationToken);
        return Task.FromResult(false);
    }

    public override Task<Customer> LookupAsync(IAsyncLookupInterfaceFilter filter, CancellationToken cancellationToken)
    {
        AsyncLookupTestLog.Record(nameof(SkippingInterfaceFilterTypedCustomerLookup), nameof(LookupAsync), filter, cancellationToken);
        throw new InvalidOperationException("LookupAsync should not be called when CanLookupAsync returns false.");
    }

    public void Dispose()
    {
        AsyncLookupTestLog.Record(nameof(SkippingInterfaceFilterTypedCustomerLookup), nameof(Dispose), null, default);
    }
}
