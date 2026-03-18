using IDCR.Abstractions.Results.Exceptions;
using System.Collections;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace IDCR.Abstractions.Results;

public interface IUnitResult : IReadOnlyDictionary<string, object?>
{
    FailureReason? FailureReason { get; }
    Exception? Exception { get; }
    bool IsSuccess { get; }
    UnitAction Action { get; }
    IUnitResult AddMeta(string key, object? value);
    IUnitResult<T> As<T>(T? value = default);
    IUnitResultCollection<T> AsCollection<T>(IEnumerable<T>? value = default);
}

public interface IUnitResult<TResult> : IUnitResult
{
    TResult? Result { get; }
    [MemberNotNullWhen(true, nameof(Result))]
    bool HasValue { get; }
}