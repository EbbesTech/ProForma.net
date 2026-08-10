using System;
using System.Collections.Generic;
using System.Text;

namespace ProForma.Common.Extensions;

public static class DictionaryExtensions
{
    extension<TKey, TValue>(IDictionary<TKey, TValue> dictionary)
    {
        public void ForEach(Action<TKey, TValue> action)
        {
            foreach (var kvp in dictionary)
            {
                action(kvp.Key, kvp.Value);
            }
        }
    }
}