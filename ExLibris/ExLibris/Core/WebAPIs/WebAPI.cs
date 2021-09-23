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

        public async Task<Response> PostAsync(string requestContent)
    => await PostAsync(string.Empty, requestContent);

        public async Task<Response> PostAsync(Dictionary<string, string> requestContent)
            => await PostAsync(string.Empty, requestContent);

        public async Task<Response> PostAsync(string requestUri, string requestContent)
            => new Response(await client.PostAsync(requestUri, GetContent(requestContent), source.Token));

        public async Task<Response> PostAsync(string requestUri, Dictionary<string, string> requestContent)
            => new Response(await client.PostAsync(requestUri, GetContent(requestContent), source.Token));

        public Response Post(string requestContent)
            => PostAsync(requestContent).Result;

        public Response Post(Dictionary<string, string> requestContent)
            => PostAsync(requestContent).Result;

        public Response Post(string requestUri, string requestContent)
            => PostAsync(requestUri, requestContent).Result;

        public Response Post(string requestUri, Dictionary<string, string> requestContent)
            => PostAsync(requestUri, requestContent).Result;


        private HttpContent GetContent(string requestContent)
            => string.IsNullOrEmpty(requestContent) ?
            null :
            new StringContent(requestContent);

        private HttpContent GetContent(Dictionary<string, string> requestContent)
            => requestContent == null || requestContent.Count == 0 ?
            null :
            new FormUrlEncodedContent(requestContent);


        public static async Task<string> ResolveUriAsync(string requestUri, Dictionary<string, string> parameters)
            => parameters == null || parameters.Count == 0 ?
            requestUri :
            $"{requestUri}?{await new FormUrlEncodedContent(parameters).ReadAsStringAsync()}";

        public static string ResolveUri(string requestUri, Dictionary<string, string> parameters)
            => ResolveUriAsync(requestUri, parameters).Result;
    }
}
