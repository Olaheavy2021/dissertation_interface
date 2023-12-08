namespace Dissertation.Application.Extensions;

public static class DictionaryExtensions
{
    public static void AddIfNotNull<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue? value)
    {
        if(value != null)
            dictionary.Add(key, value);
    }
}