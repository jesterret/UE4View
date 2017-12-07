using System.Collections.Generic;

namespace UE4View
{
    static class Extensions
    {
        public static TValue FindOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key) where TValue : new()
        {
            if (!dict.TryGetValue(key, out var val))
            {
                val = new TValue();
                dict.Add(key, val);
            }

            return val;
        }
    }
}
