using System;
using System.Collections.Generic;

namespace ExLibris.Core.Json
{
    static class JsonUtility
    {
        private const char keySeparator = '.';
        private static readonly string keySeparatorString = $"{keySeparator}";
        private const string arrayBra = "[";
        private const string arrayKet = "]";
        private const string anyKey = "*";
        public const string JsonRootKey = "";

        public static bool IsJsonRootKey(string keyPath) => string.IsNullOrEmpty(keyPath);

        public static bool IsJsonDictionaryKey(string key) => !IsJsonArrayKey(key);

        public static bool IsJsonArrayKey(string key) => key.StartsWith(arrayBra) && key.EndsWith(arrayKet);

        public static bool IsJsonAnyKey(string key) => key == anyKey;

        public static int GetJsonArrayIndex(string key) => int.Parse(key.Substring(1, key.Length - 2));

        public static string[] SplitKeyPath(string keyPath) => IsJsonRootKey(keyPath) ? null : keyPath.Split(keySeparator);

        public static (string FrontPartKey, string LastKey) SplitLastKey(string keyPath)
        {
            if (IsJsonRootKey(keyPath))
            {
                return (null, null);
            }

            var lastSepPos = keyPath.LastIndexOf(keySeparator);

            if (lastSepPos < 0)
            {
                return (null, keyPath);
            }

            return (keyPath.Substring(0, lastSepPos), keyPath.Substring(lastSepPos + 1));
        }

        public static string ConcatKey(string frontPartKey, string key)
            => IsJsonRootKey(frontPartKey) ? key : $"{frontPartKey}{keySeparator}{key}";

        public static string ConcatKey(string frontPartKey, int arrayIndex)
        => IsJsonRootKey(frontPartKey) ?
                $"{arrayBra}{arrayIndex}{arrayKet}" :
                $"{frontPartKey}{keySeparator}{arrayBra}{arrayIndex}{arrayKet}";

        public static string ConcatKey(IEnumerable<string> keys) => string.Join(keySeparatorString, keys);

        public static object NewJsonElement(string key)
            => IsJsonArrayKey(key) ? (object)new List<object>() : new Dictionary<string, object>();

        public static bool IsJsonDictionary(object obj) => obj is Dictionary<string, object>;

        public static bool IsJsonArray(object obj) => obj is List<object>;

        public static bool IsJsonDictionaryOrArray(object obj) => IsJsonDictionary(obj) || IsJsonArray(obj);

        public static Dictionary<string, object> CastJsonDictionary(object obj) => (Dictionary<string, object>)obj;

        public static List<object> CastJsonArray(object obj) => (List<object>)obj;

        public static object CreateJsonObjectFromJsonText(string jsonText, ObjectRepository objectRepository, JsonValueConverter jsonValueConverter)
        => JsonObjectSerialiser.JsonTextToJsonObject(jsonText) ??
                new JsonObjectBuilder(objectRepository, jsonValueConverter)
                .SetOnlyRootValue(jsonText)
                .BuildJsonObject();

    }
}
