using Utf8Json;

namespace ExLibris.Core.Json
{
    public static class JsonObjectSerialiser
    {
        public static string Serialize(object jsonObject) => JsonSerializer.ToJsonString(jsonObject);

        public static string ToJsonPrettyText(object jsonObject) => JsonSerializer.PrettyPrint(JsonSerializer.Serialize(jsonObject));
    }
}
