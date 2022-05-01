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

        public IEnumerable<(string KeyPath, object Value)> GetJsonValues() => GetJsonValues(int.MaxValue, JsonUtility.RootKey, jsonObject);

        public IEnumerable<(string KeyPath, object Value)> GetJsonValues(int depth) => GetJsonValues(depth, JsonUtility.RootKey, jsonObject);

        public IEnumerable<(string KeyPath, object Value)> GetJsonValues(string keyPath)
        {
            if (JsonUtility.IsRootElement(keyPath))
            {
                yield return (JsonUtility.RootKey, jsonObject);
            }
            else
            {
                var keys = JsonUtility.SplitKeyPath(keyPath);

                var kvs = GetJsonValues(JsonUtility.RootKey, keys, jsonObject);

                foreach(var kv in kvs)
                {
                    yield return kv;
                }
            }
        }

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

            if (JsonUtility.IsJsonDictionary(obj) && JsonUtility.IsJsonDictionaryKey(key))
            {
                if (JsonUtility.CastJsonDictionary(obj).TryGetValue(key, out var o))
                {
                    return o;
                }
            }
            if (JsonUtility.IsJsonArray(obj) && JsonUtility.IsJsonArrayKey(key))
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

        private static IEnumerable<(string KeyPath, object Value)> GetJsonValues(string currentKeyPath, IEnumerable<string> rest, object currentValue)
        {
            var rf = rest.FirstOrDefault();

            if (rf == null)
            {
                yield return (currentKeyPath, currentValue);
            }
            else
            {
                var rr = rest.Skip(1);

                if (JsonUtility.IsAnyKey(rf))
                {
                    foreach (var kv in GetJsonAnyValue(currentKeyPath, rr, currentValue))
                    {
                        yield return kv;
                    }
                }
                else
                {
                    if (JsonUtility.IsJsonDictionary(currentValue) && JsonUtility.IsJsonDictionaryKey(rf))
                    {
                        var kvs = GetJsonValues(JsonUtility.ConcatKey(currentKeyPath, rf), rr, JsonUtility.CastJsonDictionary(currentValue)[rf]);

                        foreach (var kv in kvs)
                        {
                            yield return kv;
                        }
                    }
                    else if(JsonUtility.IsJsonArray(currentValue) && JsonUtility.IsJsonArrayKey(rf))
                    {
                        var index = JsonUtility.GetJsonArrayIndex(rf);
                        var kvs = GetJsonValues(JsonUtility.ConcatKey(currentKeyPath, index), rr, JsonUtility.CastJsonArray(currentValue)[index]);

                        foreach (var kv in kvs)
                        {
                            yield return kv;
                        }
                    }
                }
            }
        }

        private static IEnumerable<(string KeyPath, object Value)> GetJsonAnyValue(string currentKeyPath, IEnumerable<string> restrest, object currentValue)
        {
            if (JsonUtility.IsJsonDictionary(currentValue))
            {
                var kvs = JsonUtility.CastJsonDictionary(currentValue)
                    .SelectMany(kv => GetJsonValues(JsonUtility.ConcatKey(currentKeyPath, kv.Key), restrest, kv.Value));

                foreach (var kv in kvs)
                {
                    yield return kv;
                }
            }
            else if (JsonUtility.IsJsonArray(currentValue))
            {
                var kvs = JsonUtility.CastJsonArray(currentValue)
                    .SelectMany((value, index) => GetJsonValues(JsonUtility.ConcatKey(currentKeyPath, index), restrest, value));

                foreach (var kv in kvs)
                {
                    yield return kv;
                }
            }
        }

        private static IEnumerable<(string KeyPath, object Value)> ToEnumerable(string keyPath, object value)
        {
            yield return (keyPath, value);
        }
    }
}
