using ExcelDna.Integration;
using System;
using System.Collections.Generic;

namespace ExLibris.Core.Json
{
    static class JsonUtility
    {
        private const char keySeparator = '.';
        private const string arrayBra = "[";
        private const string arrayKet = "]";
        private const string jsonObjectName = "JsonObject";

        public static bool IsRootElement(string keyPath) => string.IsNullOrEmpty(keyPath);

        public static bool IsJsonDictionaryKey(string key) => !IsJsonArrayKey(key);

        public static bool IsJsonArrayKey(string key) => key.StartsWith(arrayBra) && key.EndsWith(arrayKet);

        public static int GetJsonArrayIndex(string key) => int.Parse(key.Substring(1, key.Length - 2));

        public static string[] SplitKeyPath(string keyPath) => IsRootElement(keyPath) ? null : keyPath.Split(keySeparator);

        public static (string FrontPartKey, string LastKey) SplitLastKey(string keyPath)
        {
            if (IsRootElement(keyPath))
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
            => IsRootElement(frontPartKey) ? key : $"{frontPartKey}{keySeparator}{key}";

        public static string ConcatKey(string frontPartKey, int arrayIndex)
        => IsRootElement(frontPartKey) ?
                $"{arrayBra}{arrayIndex}{arrayKet}" :
                $"{frontPartKey}{keySeparator}{arrayBra}{arrayIndex}{arrayKet}";

        public static object NewJsonElement(string key)
            => IsJsonArrayKey(key) ? (object)new List<object>() : new Dictionary<string, object>();

        public static bool IsJsonDictionary(object obj) => obj is Dictionary<string, object>;

        public static bool IsJsonArray(object obj) => obj is List<object>;

        public static bool IsJsonDictionaryOrArray(object obj) => IsJsonDictionary(obj) || IsJsonArray(obj);

        public static Dictionary<string, object> CastJsonDictionary(object obj) => (Dictionary<string, object>)obj;

        public static List<object> CastJsonArray(object obj) => (List<object>)obj;

        public static object ObserveJsonObject(
        string callerFunctionName,
        ObjectRepository objectRepository,
        Func<object> jsonObjectFunc,
        params object[] paramObjects)
        => ExLibrisUtility.ExcelObserveObjectRegistration(
            callerFunctionName,
            jsonObjectName,
            objectRepository,
            jsonObjectFunc,
            paramObjects);

        public static object ObserveJsonObjectAsync(
        string callerFunctionName,
        ObjectRepository objectRepository,
        Func<object> jsonObjectFunc,
        params object[] paramObjects)
        => ExLibrisUtility.ExcelObserveObjectRegistrationAsync(
            callerFunctionName,
            jsonObjectName,
            objectRepository,
            jsonObjectFunc,
            paramObjects);

        public static ObjectRegistrationHandle<object> NewJsonObjectHandle(ObjectRepository objectRepository, Func<object> func)
            => new ObjectRegistrationHandle<object>(jsonObjectName, objectRepository, func);

        public static IExcelObservable NewObservableJsonObjectHandle(ObjectRepository objectRepository, Func<object> func)
            => ExLibrisUtility.NewObservableObjectRegistrationHandle(NewJsonObjectHandle(objectRepository, func));

        public static IExcelObservable NewObservableJsonObjectHandleAsync(ObjectRepository objectRepository, Func<object> func)
            => ExLibrisUtility.NewObservableObjectRegistrationHandleAsync(NewJsonObjectHandle(objectRepository, func));

        public static object CreateJsonObjectFromJsonText(string jsonText, ObjectRepository objectRepository, JsonValueConverter jsonValueConverter)
        => JsonObjectSerialiser.JsonTextToJsonObject(jsonText) ??
                new JsonObjectBuilder(objectRepository, jsonValueConverter)
                .SetOnlyRootValue(jsonText)
                .BuildJsonObject();

    }
}
