using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace ExLibris.Core.WebAPIs
{
    public class WebAPIClient : IDisposable
    {
        private readonly CancellationTokenSource source = new CancellationTokenSource();
        private readonly HttpClient client;

        private static HttpClient NewHttpClient(string baseUri, WebAPIHeaders headers)
        {
            var client = new HttpClient();

            if (!string.IsNullOrEmpty(baseUri))
            {
                client.BaseAddress = new Uri(baseUri);
            }

            if (headers == null)
            {
                return client;
            }

            var hs = client.DefaultRequestHeaders;

            if (!string.IsNullOrEmpty(headers.UserAgent))
            {
                hs.UserAgent.ParseAdd(headers.UserAgent);
            }

            hs.Authorization = GetAuthenticationHeaderValue(headers.Authorization);

            if (headers.CustomHeaders != null)
            {
                foreach(var header in headers.CustomHeaders)
                {
                    hs.TryAddWithoutValidation(header.Name, header.Value);
                }
            }

            return client;
        }

        private static AuthenticationHeaderValue GetAuthenticationHeaderValue(Authorization authorization)
        {
            if (authorization == null || string.IsNullOrEmpty(authorization.Scheme))
            {
                return null;
            }

            if (string.IsNullOrEmpty(authorization.Parameter))
            {
                return new AuthenticationHeaderValue(authorization.Scheme);
            }

            return new AuthenticationHeaderValue(authorization.Scheme, authorization.Parameter);
        }

        public WebAPIClient(string baseUri, WebAPIHeaders headers)
        {
            client = NewHttpClient(baseUri, headers);
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
    }
}
