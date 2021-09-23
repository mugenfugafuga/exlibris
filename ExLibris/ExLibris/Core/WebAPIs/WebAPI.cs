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
            => parameters == null || parameters.Count == 0 ?
            new Response(await client.GetAsync(requestUri, source.Token)) :
            new Response(await client.GetAsync($"{requestUri}?{await new FormUrlEncodedContent(parameters).ReadAsStringAsync()}", source.Token));

        public Response Get(Dictionary<string, string> parameters) => GetAsync(parameters).Result;

        public Response Get(string requestUri, Dictionary<string, string> parameters)
            => GetAsync(requestUri, parameters).Result;
    }
}
