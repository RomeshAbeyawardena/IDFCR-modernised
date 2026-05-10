using IDFCR.Results.Http.Extensions;
using System.Collections;
using System.Collections.Immutable;
using System.Text.Json;
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

        _meta.Add(Abstractions.Metadata.Meta.Action, unitResult.Action.ToString());

        if (unitResult.FailureReason.HasValue)
        {
            _meta.Add(Abstractions.Metadata.Meta.FailureReason, unitResult.FailureReason.ToString());
        }

        return _meta.ToImmutableDictionary();
    }

    [JsonPropertyName(Abstractions.Metadata.Meta.Key)]
    public IReadOnlyDictionary<string, string?> Meta => ToImmutableDictionary(unitResult.Meta);
    public bool IsSuccess { get; } = unitResult.IsSuccess;
}

internal class UnitResult<T> : UnitResult, IUnitResult<T>
{
    private readonly IReadOnlyDictionary<string, string?> results;
    public UnitResult(Abstractions.Results.IUnitResult<T> unitResult) : base(unitResult)
    {
        Dictionary<string, string?> dictionary = unitResult.HasValue
        ? new (unitResult.Result.ToDictionary())
        : new ();

        dictionary.TryAdd(Abstractions.Metadata.Meta.Key, JsonSerializer.Serialize(Meta, JsonSerializerOptions.Web));
        results = dictionary;
    }

    string? IReadOnlyDictionary<string, string?>.this[string key] => results[key];

    IEnumerable<string> IReadOnlyDictionary<string, string?>.Keys => results.Keys;
    IEnumerable<string?> IReadOnlyDictionary<string, string?>.Values => results.Values;
    int IReadOnlyCollection<KeyValuePair<string, string?>>.Count => results.Count;

    public IEnumerator<KeyValuePair<string, string?>> GetEnumerator()
    {
        return results.GetEnumerator();
    }

    bool IReadOnlyDictionary<string, string?>.ContainsKey(string key)
    {
        return results.ContainsKey(key);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    bool IReadOnlyDictionary<string, string?>.TryGetValue(string key, out string? value)
    {
        return results.TryGetValue(key, out value);
    }
}
