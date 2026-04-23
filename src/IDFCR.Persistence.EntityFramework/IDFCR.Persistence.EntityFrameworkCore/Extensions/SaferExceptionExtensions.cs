using IDFCR.Abstractions.Results;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;

namespace IDFCR.Persistence.EntityFrameworkCore.Extensions;

/// <summary>
/// Defines extension methods for configuring and adding default safer exception mappings for database-related exceptions in the safer exception provider builder. These extensions provide a convenient way to specify how common database exceptions, such as those related to SQL operations and Entity Framework Core updates, should be transformed into safer exceptions with appropriate messages, status codes, and failure reasons. By using these extension methods, developers can easily enhance the exception handling capabilities of their applications when working with databases, improving error reporting and user experience while maintaining security best practices by controlling the information exposed in exceptions.
/// </summary>
public static class SaferExceptionExtensions
{
    /// <summary>
    /// Adds default safer exception mappings for common database-related exceptions to the provided <see cref="ISaferExceptionProviderBuilder"/>. This method configures how specific exceptions, such as <see cref="SqlException"/>, <see cref="DbUpdateException"/>, and <see cref="DbUpdateConcurrencyException"/>, should be transformed into safer exceptions with predefined messages, HTTP status codes, and failure reasons. By calling this extension method, developers can ensure that their applications handle database exceptions in a consistent and secure manner, providing meaningful feedback to users while avoiding the exposure of sensitive information contained in the original exceptions.
    /// </summary>
    /// <param name="builder">The safer exception provider builder to which the default database exception mappings will be added.</param>
    /// <returns>The updated safer exception provider builder.</returns>
    public static ISaferExceptionProviderBuilder AddDatabaseExceptionDefaults(ISaferExceptionProviderBuilder builder)
    {
        return builder
            .AddOrUpdate<SqlException>("A database operation failed", 500, FailureReason.InternalError)
            .AddOrUpdate<DbUpdateException>("A data persistence error occurred", 500, FailureReason.InternalError)
            .AddOrUpdate<DbUpdateConcurrencyException>("The resource was modified by another process", 409, FailureReason.Conflict);

    }
}
