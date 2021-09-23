using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ExLibris.Core.WebAPIs
{
    public class WebAPI : IDisposable
    {
        private CancellationTokenSource source = new CancellationTokenSource();
        private HttpClient client;

        public WebAPI()
        {
            client = new HttpClient();
        }

        public WebAPI(string baseUri)
        {
            client = new HttpClient
            {
                BaseAddress = new Uri(baseUri),
            };
        }

        public void Dispose()
        {
            source.Dispose();
            client.Dispose();
        }

        public async Task<Response> GetAsync() => await GetAsync(string.Empty);

        public async Task<Response> GetAsync(string requestUri)
            => new Response(await client.GetAsync(requestUri, source.Token));

        public Response Get() => GetAsync().Result;

        public Response Get(string requestUri) => GetAsync(requestUri).Result;

        public static async Task<string> ResolveUriAsync(string requestUri, Dictionary<string, string> parameters)
            => parameters == null || parameters.Count == 0 ?
            requestUri :
            $"{requestUri}?{await new FormUrlEncodedContent(parameters).ReadAsStringAsync()}";

        public static string ResolveUri(string requestUri, Dictionary<string, string> parameters)
            => ResolveUriAsync(requestUri, parameters).Result;
    }
}
