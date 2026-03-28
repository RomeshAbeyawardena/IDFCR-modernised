using System.Collections;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

namespace BuildTools.Cli
{
    public class ArgumentParameters(IEnumerable<string> arguments) : IArgumentParameters
    {
        private readonly ReadOnlyDictionary<string, Parameter> _dictionary = new(ArgumentSplitter.Split(arguments));

        public Parameter this[string key] => _dictionary[key];

        public IEnumerable<string> Keys => _dictionary.Keys;
        public IEnumerable<Parameter> Values => _dictionary.Values;
        public int Count => _dictionary.Count;

        public bool ContainsKey(string key)
        {
            return _dictionary.ContainsKey(key);
        }

        public IEnumerator<KeyValuePair<string, Parameter>> GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }

        public bool TryGetValue(string key, [MaybeNullWhen(false)] out Parameter value)
        {
            return TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
