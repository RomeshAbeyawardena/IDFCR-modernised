namespace BuildTools.Cli
{
    internal class ArgumentSplitter
    {
        public static IDictionary<string, Parameter> Split(IEnumerable<string> arguments)
        {
            var dictionary = new Dictionary<string, Parameter>();
            var parameters = Split_internal(arguments);

            foreach (var (key, value) in parameters)
            {
                if (value is bool)
                {
                    dictionary.Add(key, new Parameter(key, IsFlag: true));
                    continue;
                }

                if (value is string)
                {
                    dictionary.Add(key, new Parameter(key, value?.ToString()));
                }
            }

            return dictionary;
        }

        private static Dictionary<string, object> Split_internal(IEnumerable<string> arguments)
        {
            var dict = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            string? currentKey = null;
            foreach (var arg in arguments)
            {
                if (arg.StartsWith("--"))
                {
                    if (currentKey != null)
                    {
                        dict[currentKey] = true; // Flag without value
                    }
                    currentKey = arg[2..].Trim('-');
                }
                else if (arg.StartsWith('-'))
                {
                    if (currentKey != null)
                    {
                        dict[currentKey] = true; // Flag without value
                    }
                    currentKey = arg[1..].Trim('-');
                }
                else
                {
                    if (currentKey != null)
                    {
                        dict[currentKey] = arg; // Key-value pair
                        currentKey = null;
                    }
                    else
                    {
                        // Positional argument without a key
                        dict[arg] = true; // Treat as a flag
                    }
                }
            }
            if (currentKey != null)
            {
                dict[currentKey] = true; // Last flag without value
            }
            return dict;
        }
    }
}
