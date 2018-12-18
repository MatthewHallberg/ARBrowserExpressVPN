using System.Collections.Generic;

namespace UnityEngine.XR.ARExtensions
{
    internal static class RegistrationHelper
    {
        public static TValue GetValueOrDefault<TKey, TValue>(Dictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue)
        {
            TValue value;
            return dictionary.TryGetValue(key, out value) ? value : defaultValue;
        }
    }
}
