using IDFCR.Abstractions.Metadata;

namespace IDFCR.Abstractions.Filters.Tests.Assets;

public sealed record AsyncLookupObjectFilter : IFilter;

public sealed record AsyncLookupTypedFilter : IFilter;

public interface IAsyncLookupInterfaceFilter : IFilter;

public sealed record AsyncLookupConcreteInterfaceFilter : IAsyncLookupInterfaceFilter;

public sealed record AsyncLookupDifferentInterfaceFilter : IAsyncLookupInterfaceFilter;
