using Exlibris.Core.JSONs.JsonNet;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;

namespace Exlibris.Core.JSONs;

public static class JSONParserManager
{
    private static readonly IJSONSerializer<JObject, JArray, JValue, JToken, JSchema> parser = new JSONNetSerializer();

    public static IJSONSerializer<JObject, JArray, JValue, JToken, JSchema> GetJSONParser() => parser;
}
