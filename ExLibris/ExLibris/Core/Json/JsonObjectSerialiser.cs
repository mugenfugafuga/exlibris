using Utf8Json;

namespace ExLibris.Core.Json
{
    public static class JsonObjectSerialiser
    {
        public static string Serialize(object jsonObject) => JsonSerializer.ToJsonString(jsonObject);

        public static string SerializePrety(object jsonObject) => JsonSerializer.PrettyPrint(JsonSerializer.Serialize(jsonObject));
    }
}
