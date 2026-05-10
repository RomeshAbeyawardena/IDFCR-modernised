using IDFCR.Results.Http.Extensions;
using Microsoft.AspNetCore.Http;
using System.Collections;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace IDFCR.Results.Http;

/// <summary>
/// Represents an HTTP result that can be returned from an API endpoint, based on an IUnitResult. It provides a method to write the response to the HttpResponse and a method to get the appropriate status code based on the failure reason of the result.
/// </summary>
public interface IUnitHttpResult : IResult
{
    /// <summary>
    /// Gets the appropriate HTTP status code based on the failure reason of the result. For example, if the failure reason is ValidationError, it returns 400 Bad Request; if it's NotFound, it returns 404 Not Found; if it's InternalError, it returns 500 Internal Server Error; and so on. If there is no failure (FailureReason.None), it returns 200 OK.
    /// </summary>
    /// <returns>The HTTP status code corresponding to the failure reason.</returns>
    int GetStatusCode();
}

internal interface IUnitResult
{
    IReadOnlyDictionary<string, string?> Meta { get; }
    bool IsSuccess { get; }
}

internal interface IUnitResult<T> : IReadOnlyDictionary<string, string?>, IUnitResult
{
    
}

internal class UnitResult(Abstractions.Results.IUnitResult unitResult) : IUnitResult
{
    private Dictionary<string, string?>? _meta = null;
    public IReadOnlyDictionary<string, string?> ToImmutableDictionary(IReadOnlyDictionary<string, object?> data)
    {
        if (_meta is not null)
        {
            return _meta;
        }

        _meta = data.ToDictionary(x => x.Key, y => y.Value?.ToString());

        _meta.Add(Abstractions.Metadata.Meta.Action, unitResult.Action.ToString());

        if (unitResult.FailureReason.HasValue)
        {
            _meta.Add(Abstractions.Metadata.Meta.FailureReason, unitResult.FailureReason.ToString());
        }

        return _meta.ToImmutableDictionary();
    }

    public IReadOnlyDictionary<string, string?> Meta => ToImmutableDictionary(unitResult.Meta);
    public bool IsSuccess { get; } = unitResult.IsSuccess;
}

internal class UnitResult<T>(Abstractions.Results.IUnitResult<T> unitResult) : UnitResult(unitResult), IUnitResult<T>
{
    private readonly IReadOnlyDictionary<string, string?> results = unitResult.HasValue 
        ? unitResult.Result.ToDictionary() 
        : new Dictionary<string,string?>();

    public string? this[string key] => results[key];

    public IEnumerable<string> Keys => results.Keys;
    public IEnumerable<string?> Values => results.Values;
    public int Count => results.Count;

    public bool ContainsKey(string key)
    {
        return results.ContainsKey(key);
    }

    public IEnumerator<KeyValuePair<string, string?>> GetEnumerator()
    {
        return results.GetEnumerator();
    }

    public bool TryGetValue(string key, [MaybeNullWhen(false)] out string? value)
    {
        return results.TryGetValue(key, out value);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}