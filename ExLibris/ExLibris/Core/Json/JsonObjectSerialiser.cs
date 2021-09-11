using Utf8Json;

namespace ExLibris.Core.Json
{
    public static class JsonObjectSerialiser
    {
        public static string ToJsonText(object jsonObject) => JsonSerializer.ToJsonString(jsonObject);

        public static string ToJsonPrettyText(object jsonObject) => JsonSerializer.PrettyPrint(JsonSerializer.Serialize(jsonObject));

        public static object ToJsonObject(string jsonText) => JsonSerializer.Deserialize<object>(jsonText);
    }
}
