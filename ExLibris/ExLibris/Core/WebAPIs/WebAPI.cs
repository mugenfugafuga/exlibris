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

        public async Task<Response> GetAsync(Dictionary<string, string> parameters)
            => await GetAsync(string.Empty, parameters);


        public async Task<Response> GetAsync(string requestUri, Dictionary<string, string> parameters)
            => new Response(await client.GetAsync(await ResolveUriAsync(requestUri, parameters), source.Token));

        public Response Get(Dictionary<string, string> parameters) => GetAsync(parameters).Result;

        public Response Get(string requestUri, Dictionary<string, string> parameters)
            => GetAsync(requestUri, parameters).Result;

        public async Task<string> ResolveUriAsync(string requestUri, Dictionary<string, string> parameters)
            => parameters == null || parameters.Count == 0 ?
            requestUri :
            $"{requestUri}?{await new FormUrlEncodedContent(parameters).ReadAsStringAsync()}";

        public string ResolveUri(string requestUri, Dictionary<string, string> parameters)
            => ResolveUriAsync(requestUri, parameters).Result;
    }
}
