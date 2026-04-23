using System.Diagnostics.CodeAnalysis;

namespace IDFCR.Abstractions.Results;

internal class DefaultSaferExceptionProvider(IReadOnlyDictionary<Type, Func<Exception, SaferException>> saferExceptionTypes) : ISaferExceptionProvider
{
    public bool TryGet<TException>(TException exception, [MaybeNullWhen(false)] out ISaferException? saferException) where TException : Exception
    {
        saferException = null;
        if (TryGetImplementation(exception, out var _saferException))
        {
            saferException = _saferException;
            return true;
        }

        return false;
    }

    public bool TryGetImplementation<TException>(TException exception, [MaybeNullWhen(false)] out SaferException? saferException) where TException : Exception
    {
        saferException = null;
        if (saferExceptionTypes.TryGetValue(typeof(TException), out var saferExceptionDelegate))
        {
            saferException = saferExceptionDelegate(exception);

            return true;
        }

        return false;
    }
}