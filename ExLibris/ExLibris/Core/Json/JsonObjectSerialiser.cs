using Utf8Json;

namespace ExLibris.Core.Json
{
    static class JsonObjectSerialiser
    {
        public static string ToJsonText(object jsonObject) => JsonSerializer.ToJsonString(jsonObject);

        public static string ToJsonPrettyText(object jsonObject) => JsonSerializer.PrettyPrint(JsonSerializer.Serialize(jsonObject));

        public static object JsonTextToJsonObject(string jsonText) => JsonSerializer.Deserialize<object>(jsonText);

        public static object ToJsonObject<T>(T obj) => JsonSerializer.Deserialize<object>(JsonSerializer.Serialize(obj));

        public static object ToJsonObject(object obj) => JsonSerializer.Deserialize<object>(JsonSerializer.NonGeneric.Serialize(obj));

        public static T ToObject<T>(object jsonOblect) => JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(jsonOblect));
    }
}
