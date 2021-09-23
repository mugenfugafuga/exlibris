using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ExLibris.Core.WebAPIs
{
    public class Response
    {
        private HttpResponseMessage httpResponse;

        public Response(HttpResponseMessage httpResponse)
        {
            this.httpResponse = httpResponse;
        }

        public async Task<string> GetContentAsStringAsync()
            => await httpResponse.Content.ReadAsStringAsync();

        public string GetContent() => GetContentAsStringAsync().Result;

        public int HttpStatus => Convert.ToInt32(httpResponse.StatusCode);
    }
}
