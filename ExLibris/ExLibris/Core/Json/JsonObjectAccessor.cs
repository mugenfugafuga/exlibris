using System.Collections.Generic;
using System.Linq;

namespace ExLibris.Core.Json
{
    public class JsonObjectAccessor
    {
        private readonly object jsonObject;

        public JsonObjectAccessor(object jsonObject)
        {
            this.jsonObject = jsonObject;
        }

        public IEnumerable<(string KeyPath, object Value)> GetJsonValues() => GetJsonValues(int.MaxValue, null, jsonObject);

        public IEnumerable<(string KeyPath, object Value)> GetJsonValues(int depth) => GetJsonValues(depth, null, jsonObject);

        public object GetJsonValue(string keyPath)
        {
            if (JsonUtility.IsRootElement(keyPath))
            {
                return jsonObject;
            }

            var keys = JsonUtility.SplitKeyPath(keyPath);

            return keys.Aggregate(jsonObject, (obj, key) => DescendJsonValue(obj, key));
        }

        private static object DescendJsonValue(object obj, string key)
        {
            if (obj == null)
            {
                return null;
            }

            if (JsonUtility.IsJsonDictionary(obj) && JsonUtility.IsJsonDictionary(key))
            {
                if (JsonUtility.CastJsonDictionary(obj).TryGetValue(key, out var o))
                {
                    return o;
                }
            }
            if (JsonUtility.IsJsonArray(obj) && JsonUtility.IsJsonArray(key))
            {
                var list = JsonUtility.CastJsonArray(obj);
                var index = JsonUtility.GetJsonArrayIndex(key);

                if (index < list.Count)
                {
                    return list[index];
                }
            }

            return null;
        }

        private static IEnumerable<(string KeyPath, object Value)> GetJsonValues(int rest, string keyPath, object value)
        {
            if (rest > 0 && JsonUtility.IsJsonDictionary(value))
            {
                return JsonUtility.CastJsonDictionary(value)
                    .SelectMany(kv => GetJsonValues(rest - 1, JsonUtility.ConcatKey(keyPath, kv.Key), kv.Value));
            }
            else if (rest > 0 && JsonUtility.IsJsonArray(value))
            {
                return JsonUtility.CastJsonArray(value)
                    .SelectMany((v, index) => GetJsonValues(rest - 1, JsonUtility.ConcatKey(keyPath, index), v));
            }
            else
            {
                return ToEnumerable(keyPath, value);
            }
        }

        private static IEnumerable<(string KeyPath, object Value)> ToEnumerable(string keyPath, object value)
        {
            yield return (keyPath, value);
        }
    }
}
