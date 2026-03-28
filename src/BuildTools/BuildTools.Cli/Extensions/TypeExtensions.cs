namespace BuildTools.Cli.Extensions;

public static class TypeExtensions
{
   public static bool HasInterface<TInterface>(this Type type, params Type[] excludedTypes)
    {
        return type.GetInterface(typeof(TInterface).Name) is not null
            && excludedTypes.All(x => x != type);
    }
}
