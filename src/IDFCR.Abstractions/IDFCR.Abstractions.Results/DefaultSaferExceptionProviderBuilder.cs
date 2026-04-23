using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text.Json;
using System.Xml;

namespace IDFCR.Abstractions.Results;

internal class DefaultSaferExceptionProviderBuilder : ISaferExceptionProviderBuilder
{
    private readonly Dictionary<Type, Func<Exception, SaferException>> saferExceptionTypes = [];

    public ISaferExceptionProviderBuilder AddDefaults()
    {
        return AddOrUpdate<MethodAccessException>("Unable to access a method or field", 500, FailureReason.InternalError)
            // Assembly / Runtime loading
            .AddOrUpdate<DllNotFoundException>("A required system component is missing", 500, FailureReason.InternalError)
            .AddOrUpdate<TypeLoadException>("A system type could not be loaded", 500, FailureReason.InternalError)
            .AddOrUpdate<ReflectionTypeLoadException>("One or more system components failed to load", 500, FailureReason.InternalError)
            .AddOrUpdate<MissingMethodException>("A required system method is unavailable", 500, FailureReason.InternalError)

            // Networking
            .AddOrUpdate<HttpRequestException>("An external service request failed", 502, FailureReason.ExternalDependencyError)
            .AddOrUpdate<WebException>("An external service communication error occurred", 502, FailureReason.ExternalDependencyError)

            // File system
            .AddOrUpdate<FileNotFoundException>("A required file could not be found", 500, FailureReason.InternalError)
            .AddOrUpdate<DirectoryNotFoundException>("A required directory could not be found", 500, FailureReason.InternalError)
            .AddOrUpdate<IOException>("A file system error occurred", 500, FailureReason.InternalError)
            .AddOrUpdate<UnauthorizedAccessException>("Access to a required resource was denied", 403, FailureReason.AuthorizationError)

            // Security / Crypto
            .AddOrUpdate<CryptographicException>("A security operation failed", 500, FailureReason.InternalError)

            // Serialization / Parsing
            .AddOrUpdate<JsonException>("Invalid data format received", 400, FailureReason.ValidationError)
            .AddOrUpdate<XmlException>("Invalid XML data received", 400, FailureReason.ValidationError)

            // Core runtime (be careful here)
            .AddOrUpdate<ArgumentException>("One or more arguments are invalid", 400, FailureReason.ValidationError)
            .AddOrUpdate<InvalidOperationException>("The operation could not be completed", 500, FailureReason.InternalError)
            .AddOrUpdate<NullReferenceException>("An unexpected error occurred", 500, FailureReason.InternalError);

    }

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