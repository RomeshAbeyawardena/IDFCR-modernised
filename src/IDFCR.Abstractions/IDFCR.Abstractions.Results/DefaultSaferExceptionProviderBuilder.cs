namespace IDFCR.Abstractions.Results;

internal class DefaultSaferExceptionProviderBuilder : ISaferExceptionProviderBuilder
{
    private Dictionary<Type, Func<Exception, SaferException>> saferExceptionTypes = [];
    public ISaferExceptionProviderBuilder AddOrUpdate<TException>(string saferMessage, int? statusCode, FailureReason? failureReason) where TException : Exception
    {
        var exceptionType = typeof(TException);
        
        SaferException @delegate(Exception ex) => new(ex, saferMessage, statusCode, failureReason);

        if (!saferExceptionTypes.TryAdd(exceptionType, @delegate))
        {
            if (saferExceptionTypes.ContainsKey(exceptionType))
            {
                saferExceptionTypes[exceptionType] = @delegate;
            }
        }

        return this;
    }

    public ISaferExceptionProvider Build()
    {
        return new DefaultSaferExceptionProvider(saferExceptionTypes.AsReadOnly());
    }
}