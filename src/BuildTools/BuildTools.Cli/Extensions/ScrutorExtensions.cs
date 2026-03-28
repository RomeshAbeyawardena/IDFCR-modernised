using Scrutor;

namespace BuildTools.Cli.Extensions;

public static class ScrutorExtensions
{
    public static IImplementationTypeFilter HasInterface<TInterface>(this IImplementationTypeFilter typeFilter, params Type[] excludedTypes)
    {
        return typeFilter.Where(x => x.HasInterface<TInterface>(excludedTypes));
    }

}
