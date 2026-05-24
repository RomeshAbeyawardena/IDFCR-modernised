using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using System.Security.Claims;

namespace IDFCR.Abstractions.RequestState.Http.Extensions;

/// <summary>
/// Defines extension methods for registering HTTP request state services in the dependency injection container, enabling access to request context and authenticated context within ASP.NET Core applications.
/// </summary>
public static class ServiceCollectionExtensions
{
    private static DefaultAuthenticatedContext GetAuthenticatedRequestContext(IHttpContextAccessor accessor)
    {
        var context = accessor.HttpContext ?? throw new InvalidOperationException("HttpContext is not available.");

        var user = context.User;
        bool isAuthenticated = user.Identity is not null && user.Identity.IsAuthenticated;
        Dictionary<string, object?> claims = isAuthenticated ? user.Claims.ToDictionary(x => x.Subject?.Name ?? throw new KeyNotFoundException(),
            y => (object?)y)
            : [];

        Dictionary<string, object?> headers = context.Request.Headers.ToDictionary(x => x.Key, x => (object?)x.Value);

        return new DefaultAuthenticatedContext(isAuthenticated,
            headers,
            x => x is StringValues value ? string.Join(',', value!) : string.Empty,
            claims,
            x => x is Claim claim ? claim.Value : string.Empty);
    }

    /// <summary>
    /// Adds HTTP request state services to the dependency injection container, allowing for the retrieval of request context and authenticated context information within ASP.NET Core applications. This method registers the necessary services to access the current HTTP context and extract relevant information such as authentication status, claims, and headers for use throughout the application.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to which the services will be added.</param>
    /// <returns>The updated <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddHttpRequestStateServices(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        return services
            .AddScoped<IRequestContext>(s =>
            {
                var accessor = s.GetRequiredService<IHttpContextAccessor>();
                return GetAuthenticatedRequestContext(accessor);
            })
        .AddScoped<IAuthenticatedContext>(s =>
        {
            var accessor = s.GetRequiredService<IHttpContextAccessor>();
            return GetAuthenticatedRequestContext(accessor);
        });
    }
}
