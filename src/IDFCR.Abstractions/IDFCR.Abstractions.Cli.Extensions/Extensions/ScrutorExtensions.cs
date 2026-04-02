using IDFCR.Abstractions.Cli.Extensions;
using Scrutor;

namespace IDFCR.Abstractions.Cli.Extensions
{
    public static class ScrutorExtensions
    {
        public static IImplementationTypeFilter HasInterface<TInterface>(this IImplementationTypeFilter typeFilter, params Type[] excludedTypes)
        {
            return typeFilter.Where(x => x.HasInterface<TInterface>(excludedTypes));
        }

    }
}
