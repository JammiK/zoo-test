using System.Collections.Generic;

namespace Core.JG.Extensions
{
    public static class DictionaryExtension
    {
        public static TU TryGet<T, TU>(this IDictionary<T, TU> dictionary, T key, TU defaultValue)
        {
            if (dictionary.ContainsKey(key))
            {
                return dictionary[key];
            }

            return defaultValue;
        }
        
        public static TU TryGet<T, TU>(this IDictionary<T, TU> dictionary, T key)
        {
            return dictionary.TryGet(key, default);
        }
    }
}