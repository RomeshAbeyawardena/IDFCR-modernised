using System.Reflection;

using IDFCR.Abstractions.Filters.Tests.Assets;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace IDFCR.Abstractions.Filters.Tests;

[TestFixture]
internal sealed class AsyncLookupTests
{
    [SetUp]
    public void SetUp()
    {
        AsyncLookupTestLog.Reset();
    }

    [Test]
    public async Task LookupAsync_WithNoRegisteredLookups_ReturnsEmpty()
    {
        using var serviceProvider = BuildServiceProvider();
        var factory = CreateFactory(serviceProvider);

        var results = await factory.LookupAsync<Customer>(new AsyncLookupObjectFilter(), CancellationToken.None);

        Assert.That(results, Is.Empty);
        Assert.That(AsyncLookupTestLog.Events, Is.Empty);
    }

    [Test]
    public async Task LookupAsync_ObjectOverload_ReturnsMatchesInRegistrationOrder()
    {
        using var serviceProvider = BuildServiceProvider(services =>
        {
            services.AddScoped<IAsyncLookup<Customer>, FirstObjectCustomerLookup>();
            services.AddScoped<IAsyncLookup<Customer>, SkippingObjectCustomerLookup>();
            services.AddScoped<IAsyncLookup<Customer>, SecondObjectCustomerLookup>();
        });

        var factory = CreateFactory(serviceProvider);
        var filter = new AsyncLookupObjectFilter();

        var results = (await factory.LookupAsync<Customer>(filter, CancellationToken.None)).ToArray();

        Assert.That(results, Has.Length.EqualTo(2));
        Assert.That(results.Select(customer => customer.FirstName), Is.EqualTo(new[] { "First", "Second" }));
        Assert.That(GetOperationSequence(includeDisposes: false), Is.EqualTo(new[]
        {
            $"{nameof(FirstObjectCustomerLookup)}.{nameof(IAsyncLookup<Customer>.CanLookupAsync)}",
            $"{nameof(FirstObjectCustomerLookup)}.{nameof(IAsyncLookup<Customer>.LookupAsync)}",
            $"{nameof(SkippingObjectCustomerLookup)}.{nameof(IAsyncLookup<Customer>.CanLookupAsync)}",
            $"{nameof(SecondObjectCustomerLookup)}.{nameof(IAsyncLookup<Customer>.CanLookupAsync)}",
            $"{nameof(SecondObjectCustomerLookup)}.{nameof(IAsyncLookup<Customer>.LookupAsync)}"
        }));
        Assert.That(GetDisposeCount(nameof(FirstObjectCustomerLookup)), Is.EqualTo(1));
        Assert.That(GetDisposeCount(nameof(SkippingObjectCustomerLookup)), Is.EqualTo(1));
        Assert.That(GetDisposeCount(nameof(SecondObjectCustomerLookup)), Is.EqualTo(1));
    }

    [Test]
    public async Task LookupAsync_TypedOverload_ReturnsMatchesInRegistrationOrder()
    {
        using var serviceProvider = BuildServiceProvider(services =>
        {
            services.AddScoped<IAsyncLookup<Customer, AsyncLookupTypedFilter>, FirstTypedCustomerLookup>();
            services.AddScoped<IAsyncLookup<Customer, AsyncLookupTypedFilter>, SkippingTypedCustomerLookup>();
            services.AddScoped<IAsyncLookup<Customer, AsyncLookupTypedFilter>, SecondTypedCustomerLookup>();
        });

        var factory = CreateFactory(serviceProvider);
        var filter = new AsyncLookupTypedFilter();

        var results = (await factory.LookupAsync<Customer, AsyncLookupTypedFilter>(filter, CancellationToken.None)).ToArray();

        Assert.That(results, Has.Length.EqualTo(2));
        Assert.That(results.Select(customer => customer.FirstName), Is.EqualTo(new[] { "First", "Second" }));
        Assert.That(GetOperationSequence(includeDisposes: false), Is.EqualTo(new[]
        {
            $"{nameof(FirstTypedCustomerLookup)}.{nameof(IAsyncLookup<Customer>.CanLookupAsync)}",
            $"{nameof(FirstTypedCustomerLookup)}.{nameof(IAsyncLookup<Customer, AsyncLookupTypedFilter>.LookupAsync)}",
            $"{nameof(SkippingTypedCustomerLookup)}.{nameof(IAsyncLookup<Customer>.CanLookupAsync)}",
            $"{nameof(SecondTypedCustomerLookup)}.{nameof(IAsyncLookup<Customer>.CanLookupAsync)}",
            $"{nameof(SecondTypedCustomerLookup)}.{nameof(IAsyncLookup<Customer, AsyncLookupTypedFilter>.LookupAsync)}"
        }));
        Assert.That(GetDisposeCount(nameof(FirstTypedCustomerLookup)), Is.EqualTo(1));
        Assert.That(GetDisposeCount(nameof(SkippingTypedCustomerLookup)), Is.EqualTo(1));
        Assert.That(GetDisposeCount(nameof(SecondTypedCustomerLookup)), Is.EqualTo(1));
    }

    [Test]
    public async Task LookupAsync_ObjectOverload_WithNullFilter_ReturnsEmptyAndDoesNotInvokeLookup()
    {
        using var serviceProvider = BuildServiceProvider(services =>
        {
            services.AddScoped<IAsyncLookup<Customer>, FirstObjectCustomerLookup>();
        });

        var factory = CreateFactory(serviceProvider);

        var results = await factory.LookupAsync<Customer>(null!, CancellationToken.None);

        Assert.That(results, Is.Empty);
        Assert.That(GetOperationSequence(includeDisposes: false), Is.EqualTo(new[]
        {
            $"{nameof(FirstObjectCustomerLookup)}.{nameof(IAsyncLookup<Customer>.CanLookupAsync)}"
        }));
        Assert.That(GetDisposeCount(nameof(FirstObjectCustomerLookup)), Is.EqualTo(1));
    }

    [Test]
    public async Task LookupAsync_TypedOverload_WithNullFilter_ReturnsEmptyAndDoesNotInvokeLookup()
    {
        using var serviceProvider = BuildServiceProvider(services =>
        {
            services.AddScoped<IAsyncLookup<Customer, AsyncLookupTypedFilter>, FirstTypedCustomerLookup>();
        });

        var factory = CreateFactory(serviceProvider);
        AsyncLookupTypedFilter? filter = null;

        var results = await factory.LookupAsync<Customer, AsyncLookupTypedFilter>(filter!, CancellationToken.None);

        Assert.That(results, Is.Empty);
        Assert.That(GetOperationSequence(includeDisposes: false), Is.EqualTo(new[]
        {
            $"{nameof(FirstTypedCustomerLookup)}.{nameof(IAsyncLookup<Customer>.CanLookupAsync)}"
        }));
        Assert.That(GetDisposeCount(nameof(FirstTypedCustomerLookup)), Is.EqualTo(1));
    }

    [Test]
    public async Task LookupAsync_ObjectOverload_PropagatesCancellationTokenToLookups()
    {
        using var serviceProvider = BuildServiceProvider(services =>
        {
            services.AddScoped<IAsyncLookup<Customer>, FirstObjectCustomerLookup>();
        });

        var factory = CreateFactory(serviceProvider);
        using var cts = new CancellationTokenSource();

        await factory.LookupAsync<Customer>(new AsyncLookupObjectFilter(), cts.Token);

        var canLookupEvent = GetEvent(nameof(FirstObjectCustomerLookup), nameof(IAsyncLookup<Customer>.CanLookupAsync));
        var lookupEvent = GetEvent(nameof(FirstObjectCustomerLookup), nameof(IAsyncLookup<Customer>.LookupAsync));

        Assert.That(canLookupEvent.CancellationToken, Is.EqualTo(cts.Token));
        Assert.That(lookupEvent.CancellationToken, Is.EqualTo(cts.Token));
    }

    [Test]
    public async Task LookupAsync_TypedOverload_PropagatesCancellationTokenToLookups()
    {
        using var serviceProvider = BuildServiceProvider(services =>
        {
            services.AddScoped<IAsyncLookup<Customer, AsyncLookupTypedFilter>, FirstTypedCustomerLookup>();
        });

        var factory = CreateFactory(serviceProvider);
        using var cts = new CancellationTokenSource();

        await factory.LookupAsync<Customer, AsyncLookupTypedFilter>(new AsyncLookupTypedFilter(), cts.Token);

        var canLookupEvent = GetEvent(nameof(FirstTypedCustomerLookup), nameof(IAsyncLookup<Customer>.CanLookupAsync));
        var lookupEvent = GetEvent(nameof(FirstTypedCustomerLookup), nameof(IAsyncLookup<Customer, AsyncLookupTypedFilter>.LookupAsync));

        Assert.That(canLookupEvent.CancellationToken, Is.EqualTo(cts.Token));
        Assert.That(lookupEvent.CancellationToken, Is.EqualTo(cts.Token));
    }

    [Test]
    public async Task LookupAsync_ObjectOverload_DoesNotInvokeLookupWhenCanLookupReturnsFalse()
    {
        using var serviceProvider = BuildServiceProvider(services =>
        {
            services.AddScoped<IAsyncLookup<Customer>, SkippingObjectCustomerLookup>();
        });

        var factory = CreateFactory(serviceProvider);

        var results = await factory.LookupAsync<Customer>(new AsyncLookupObjectFilter(), CancellationToken.None);

        Assert.That(results, Is.Empty);
        Assert.That(GetOperationSequence(includeDisposes: false), Is.EqualTo(new[]
        {
            $"{nameof(SkippingObjectCustomerLookup)}.{nameof(IAsyncLookup<Customer>.CanLookupAsync)}"
        }));
        Assert.That(GetDisposeCount(nameof(SkippingObjectCustomerLookup)), Is.EqualTo(1));
    }

    [Test]
    public async Task LookupAsync_TypedOverload_DoesNotInvokeLookupWhenCanLookupReturnsFalse()
    {
        using var serviceProvider = BuildServiceProvider(services =>
        {
            services.AddScoped<IAsyncLookup<Customer, AsyncLookupTypedFilter>, SkippingTypedCustomerLookup>();
        });

        var factory = CreateFactory(serviceProvider);

        var results = await factory.LookupAsync<Customer, AsyncLookupTypedFilter>(new AsyncLookupTypedFilter(), CancellationToken.None);

        Assert.That(results, Is.Empty);
        Assert.That(GetOperationSequence(includeDisposes: false), Is.EqualTo(new[]
        {
            $"{nameof(SkippingTypedCustomerLookup)}.{nameof(IAsyncLookup<Customer>.CanLookupAsync)}"
        }));
        Assert.That(GetDisposeCount(nameof(SkippingTypedCustomerLookup)), Is.EqualTo(1));
    }

    [Test]
    public async Task LookupAsync_ObjectOverload_ThrowsWhenAProviderThrowsAndStillDisposesScope()
    {
        using var serviceProvider = BuildServiceProvider(services =>
        {
            services.AddScoped<IAsyncLookup<Customer>, ThrowingObjectCustomerLookup>();
        });

        var factory = CreateFactory(serviceProvider);

        Assert.That(async () => await factory.LookupAsync<Customer>(new AsyncLookupObjectFilter(), CancellationToken.None),
            Throws.TypeOf<InvalidOperationException>().With.Message.EqualTo("Object lookup failure."));
        Assert.That(GetDisposeCount(nameof(ThrowingObjectCustomerLookup)), Is.EqualTo(1));
        Assert.That(GetOperationSequence(includeDisposes: true), Is.EqualTo(new[]
        {
            $"{nameof(ThrowingObjectCustomerLookup)}.{nameof(IAsyncLookup<Customer>.CanLookupAsync)}",
            $"{nameof(ThrowingObjectCustomerLookup)}.{nameof(IAsyncLookup<Customer>.LookupAsync)}",
            $"{nameof(ThrowingObjectCustomerLookup)}.{nameof(IDisposable.Dispose)}"
        }));
    }

    [Test]
    public async Task LookupAsync_TypedOverload_ThrowsWhenAProviderThrowsAndStillDisposesScope()
    {
        using var serviceProvider = BuildServiceProvider(services =>
        {
            services.AddScoped<IAsyncLookup<Customer, AsyncLookupTypedFilter>, ThrowingTypedCustomerLookup>();
        });

        var factory = CreateFactory(serviceProvider);

        Assert.That(async () => await factory.LookupAsync<Customer, AsyncLookupTypedFilter>(new AsyncLookupTypedFilter(), CancellationToken.None),
            Throws.TypeOf<InvalidOperationException>().With.Message.EqualTo("Typed lookup failure."));
        Assert.That(GetDisposeCount(nameof(ThrowingTypedCustomerLookup)), Is.EqualTo(1));
        Assert.That(GetOperationSequence(includeDisposes: true), Is.EqualTo(new[]
        {
            $"{nameof(ThrowingTypedCustomerLookup)}.{nameof(IAsyncLookup<Customer>.CanLookupAsync)}",
            $"{nameof(ThrowingTypedCustomerLookup)}.{nameof(IAsyncLookup<Customer, AsyncLookupTypedFilter>.LookupAsync)}",
            $"{nameof(ThrowingTypedCustomerLookup)}.{nameof(IDisposable.Dispose)}"
        }));
    }

    [Test]
    public async Task LookupAsync_ObjectOverload_PropagatesCanLookupExceptionsAndDisposesScope()
    {
        using var serviceProvider = BuildServiceProvider(services =>
        {
            services.AddScoped<IAsyncLookup<Customer>, ThrowingCanLookupObjectCustomerLookup>();
        });

        var factory = CreateFactory(serviceProvider);

        Assert.That(async () => await factory.LookupAsync<Customer>(new AsyncLookupObjectFilter(), CancellationToken.None),
            Throws.TypeOf<InvalidOperationException>().With.Message.EqualTo("Object can-lookup failure."));
        Assert.That(GetDisposeCount(nameof(ThrowingCanLookupObjectCustomerLookup)), Is.EqualTo(1));
        Assert.That(GetOperationSequence(includeDisposes: true), Is.EqualTo(new[]
        {
            $"{nameof(ThrowingCanLookupObjectCustomerLookup)}.{nameof(IAsyncLookup<Customer>.CanLookupAsync)}",
            $"{nameof(ThrowingCanLookupObjectCustomerLookup)}.{nameof(IDisposable.Dispose)}"
        }));
    }

    [Test]
    public async Task LookupAsync_TypedOverload_PropagatesCanLookupExceptionsAndDisposesScope()
    {
        using var serviceProvider = BuildServiceProvider(services =>
        {
            services.AddScoped<IAsyncLookup<Customer, AsyncLookupTypedFilter>, ThrowingCanLookupTypedCustomerLookup>();
        });

        var factory = CreateFactory(serviceProvider);

        Assert.That(async () => await factory.LookupAsync<Customer, AsyncLookupTypedFilter>(new AsyncLookupTypedFilter(), CancellationToken.None),
            Throws.TypeOf<InvalidOperationException>().With.Message.EqualTo("Typed can-lookup failure."));
        Assert.That(GetDisposeCount(nameof(ThrowingCanLookupTypedCustomerLookup)), Is.EqualTo(1));
        Assert.That(GetOperationSequence(includeDisposes: true), Is.EqualTo(new[]
        {
            $"{nameof(ThrowingCanLookupTypedCustomerLookup)}.{nameof(IAsyncLookup<Customer>.CanLookupAsync)}",
            $"{nameof(ThrowingCanLookupTypedCustomerLookup)}.{nameof(IDisposable.Dispose)}"
        }));
    }

    [Test]
    public void AsyncLookupBase_ObjectLookup_WithInvalidFilterType_ThrowsArgumentException()
    {
        using var lookup = new FirstTypedCustomerLookup();
        var asyncLookup = (IAsyncLookup<Customer>)lookup;

        Assert.That(async () => await asyncLookup.LookupAsync(new AsyncLookupObjectFilter(), CancellationToken.None),
            Throws.TypeOf<ArgumentException>().With.Message.Contain($"Expected {nameof(AsyncLookupTypedFilter)}"));
    }

    [Test]
    public async Task AsyncLookupBase_CanLookupAsync_RecognizesOnlyMatchingTypedFilters()
    {
        using var lookup = new FirstTypedCustomerLookup();

        var matches = await lookup.CanLookupAsync(new AsyncLookupTypedFilter(), CancellationToken.None);
        var mismatchedFilter = await lookup.CanLookupAsync(new AsyncLookupObjectFilter(), CancellationToken.None);
        var nullFilter = await lookup.CanLookupAsync(null, CancellationToken.None);

        Assert.That(matches, Is.True);
        Assert.That(mismatchedFilter, Is.False);
        Assert.That(nullFilter, Is.False);
        Assert.That(GetOperationSequence(includeDisposes: false), Is.EqualTo(new[]
        {
            $"{nameof(FirstTypedCustomerLookup)}.{nameof(IAsyncLookup<Customer>.CanLookupAsync)}",
            $"{nameof(FirstTypedCustomerLookup)}.{nameof(IAsyncLookup<Customer>.CanLookupAsync)}",
            $"{nameof(FirstTypedCustomerLookup)}.{nameof(IAsyncLookup<Customer>.CanLookupAsync)}"
        }));
    }

    private static ServiceProvider BuildServiceProvider(Action<IServiceCollection>? configure = null)
    {
        var services = new ServiceCollection();
        configure?.Invoke(services);
        return services.BuildServiceProvider();
    }

    private static IAsyncLookupFactory CreateFactory(IServiceProvider serviceProvider)
    {
        var factoryType = typeof(IAsyncLookupFactory).Assembly.GetType("IDFCR.Abstractions.Filters.AsyncLookupFactory", throwOnError: true)!;
        var constructor = factoryType.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, binder: null, [typeof(IServiceProvider)], modifiers: null)
            ?? throw new MissingMethodException(factoryType.FullName, ".ctor(IServiceProvider)");

        return (IAsyncLookupFactory)constructor.Invoke([serviceProvider]);
    }

    private static IReadOnlyList<string> GetOperationSequence(bool includeDisposes)
    {
        return AsyncLookupTestLog.Events
            .Where(eventItem => includeDisposes || eventItem.Operation != nameof(IDisposable.Dispose))
            .Select(eventItem => $"{eventItem.Source}.{eventItem.Operation}")
            .ToArray();
    }

    private static int GetDisposeCount(string source)
    {
        return AsyncLookupTestLog.Events.Count(eventItem => eventItem.Source == source && eventItem.Operation == nameof(IDisposable.Dispose));
    }

    private static AsyncLookupEvent GetEvent(string source, string operation)
    {
        return AsyncLookupTestLog.Events.Single(eventItem => eventItem.Source == source && eventItem.Operation == operation);
    }
}
