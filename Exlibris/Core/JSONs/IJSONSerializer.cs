using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;

namespace Exlibris.Core.JSONs;
public interface IJSONSerializer : IJSONSerializer<JObject, JArray, JValue, JToken, JSchema>
{
}
