using Newtonsoft.Json.Linq;

namespace Exlibris.Core.JSONs.Extension
{
    public static class JSONExtensions
    {
        public static object GetValueOrThis(this JToken token)
            => token is JValue value ? value.Value : token;

        public static JToken GetBase<T>(this T json) where T : JToken => json;
    }
}