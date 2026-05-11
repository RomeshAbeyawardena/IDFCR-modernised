using IDFCR.Results.Http.Extensions;
using System.Collections;
using System.Collections.Immutable;
using System.Text.Json.Serialization;

namespace IDFCR.Results.Http;

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

        _meta.Add(Abstractions.Metadata.Meta.ActionKey, unitResult.Action.ToString());

        if (unitResult.FailureReason.HasValue)
        {
            _meta.Add(Abstractions.Metadata.Meta.FailureReason, unitResult.FailureReason.ToString());
        }

        return _meta.ToImmutableDictionary();
    }

    [JsonPropertyName(Abstractions.Metadata.Meta.Key)]
    public IReadOnlyDictionary<string, string?> Meta => ToImmutableDictionary(unitResult.Meta);

    [JsonPropertyName(Abstractions.Metadata.Meta.SuccessKey)]
    public bool IsSuccess { get; } = unitResult.IsSuccess;
}

internal class UnitResult<T> : UnitResult, IUnitResult<T>
{
    private readonly IReadOnlyDictionary<string, object?> results;

    protected virtual void AppendToSelf(Dictionary<string, object?> source)
    {
        
    }

    public UnitResult(
        Abstractions.Results.IUnitResult? unitResult = null,
        Abstractions.Results.IUnitResult<T>? unitResultWithValue = null) : base(unitResult 
            ?? unitResultWithValue 
            ?? throw new ArgumentNullException(nameof(unitResult), "Must provide at least one type of unit result"))
    {
        Dictionary<string, object?> dictionary = [];

        if (unitResultWithValue is not null
            && unitResultWithValue.HasValue)
        {
            dictionary = new(unitResultWithValue.Result.ToDictionary(unitResultWithValue.NamedResult));
        }

        dictionary.TryAdd(Abstractions.Metadata.Meta.SuccessKey, (unitResult 
            ?? unitResultWithValue
            ?? throw new ArgumentNullException(nameof(unitResult), "Must provide at least one type of unit result")).IsSuccess);
        dictionary.TryAdd(Abstractions.Metadata.Meta.Key, Meta);
        AppendToSelf(dictionary);
        results = dictionary;
    }

    object? IReadOnlyDictionary<string, object?>.this[string key] => results[key];

    IEnumerable<string> IReadOnlyDictionary<string, object?>.Keys => results.Keys;
    IEnumerable<object?> IReadOnlyDictionary<string, object?>.Values => results.Values;
    int IReadOnlyCollection<KeyValuePair<string, object?>>.Count => results.Count;

    public IEnumerator<KeyValuePair<string, object?>> GetEnumerator()
    {
        return results.GetEnumerator();
    }

    bool IReadOnlyDictionary<string, object?>.ContainsKey(string key)
    {
        return results.ContainsKey(key);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    bool IReadOnlyDictionary<string, object?>.TryGetValue(string key, out object? value)
    {
        return results.TryGetValue(key, out value);
    }
}
