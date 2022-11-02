using System.Collections;

namespace AptamerRunner;

public static class DictionaryExtensions
{
    public static string GetStringOrDefault(this IDictionary dictionary, string key, string defaultValue)
    {
        if (dictionary.Contains(key))
        {
            return dictionary[key] as string ?? defaultValue;
        }

        return defaultValue;
    }
}