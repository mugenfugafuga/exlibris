using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace ExLibris.Core.WebAPIs
{
    public static class WebAPIUtility
    {
        public static string ResolveUri(string requestUri, Dictionary<string, string> parameters)
            => ResolveUriAsync(requestUri, parameters).Result;

        public static async Task<string> ResolveUriAsync(string requestUri, Dictionary<string, string> parameters)
            => parameters == null || parameters.Count == 0 ?
            requestUri :
            $"{requestUri}?{await new FormUrlEncodedContent(parameters).ReadAsStringAsync()}";
    }
}
