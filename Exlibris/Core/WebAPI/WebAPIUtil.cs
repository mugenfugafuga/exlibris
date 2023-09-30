using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Exlibris.Core.WebAPI
{
    public static class WebAPIUtil
    {
        private static readonly string UserAgent;

        static WebAPIUtil()
        {
            var assembly = typeof(ExlibrisAddin).Assembly;
            UserAgent = $"{assembly.GetName().Name}/{assembly.GetName().Version}";
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        }

        public static HttpClient NewClinet()
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.UserAgent.ParseAdd(UserAgent);
            return client;
        }

        public static HttpClient NewClinet(HttpClientSetting setting)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.UserAgent.ParseAdd(UserAgent);

            UpdateHeaders(client.DefaultRequestHeaders, setting.Headers);

            if (setting.BaseAddress != null) { client.BaseAddress = new Uri(setting.BaseAddress); }
            if (setting.Timeout != null) { client.Timeout = TimeSpan.Parse(setting.Timeout); }
            if (setting.MaxResponseContentBufferSize != null) { client.MaxResponseContentBufferSize = setting.MaxResponseContentBufferSize.Value; }

            return client;
        }

        public static HttpRequestMessage CreateRequest(HttpMethod method, string uri, HttpContent content)
            => new HttpRequestMessage(method, uri) { Content = content };

        public static HttpRequestMessage CreateRequest(HttpMethod method, string uri, HttpRequestSetting setting, HttpContent content)
        {
            var request = new HttpRequestMessage(method, uri) { Content = content };

            UpdateRequest(request, setting);

            return request;
        }

        public static HttpRequestMessage CreateRequest(HttpMethod method, string uri, AuthenticationHeaderValue authentication, HttpContent content)
        {
            var request = new HttpRequestMessage(method, uri) { Content = content };

            request.Headers.Authorization = authentication;

            return request;
        }

        public static void UpdateRequest(HttpRequestMessage request, HttpRequestSetting setting)
        {
            if (setting == null) { return; }

            request.Headers.UserAgent.ParseAdd(UserAgent);

            UpdateHeaders(request.Headers, setting.Headers);

            if (setting.Version != null) { request.Version = Version.Parse(setting.Version); }
        }

        public static void UpdateHeaders(HttpRequestHeaders headers, HttpHeaders setting)
        {
            foreach (var header in setting.Headers)
            {
                if (!string.IsNullOrEmpty(header.Key))
                {
                    headers.Add(header.Key, header.Values.ToArray());
                }
            }

            headers.Authorization = CreateHeader(setting.AuthenticationHeader);
        }

        public static AuthenticationHeaderValue CreateHeader(BasicAuthentication basic)
        {
            if (basic != null)
            {
                if (!string.IsNullOrEmpty(basic.User) && !string.IsNullOrEmpty(basic.Password))
                {
                    return new AuthenticationHeaderValue("Basic", ToBASE64(basic.User, basic.Password));
                }

                if (!string.IsNullOrEmpty(basic.Base64))
                {
                    return new AuthenticationHeaderValue("Basic", basic.Base64);
                }
            }

            return null;
        }

        public static AuthenticationHeaderValue CreateHeader(Bearer bearer)
        {
            if (bearer != null)
            {
                if (!string.IsNullOrEmpty(bearer.AccessToken))
                {
                    return new AuthenticationHeaderValue("Bearer", bearer.AccessToken);
                }
            }

            return null;
        }

        public static AuthenticationHeaderValue CreateHeader(HttpAuthenticationHeader header)
            => header != null ?
                CreateHeader(header.Basic) ??
                CreateHeader(header.Bearer) ??
                CreateHeader(header.Schema, header.Certification) : null;

        private static AuthenticationHeaderValue CreateHeader(string schema, string certification)
            => !string.IsNullOrEmpty(schema) && !string.IsNullOrEmpty(certification) ?
                new AuthenticationHeaderValue(schema, certification) : null;

        public static string ToBASE64(string account, string password)
            => ToBASE64($"{account}:{password}");

        private static string ToBASE64(string value)
            => Convert.ToBase64String(Encoding.ASCII.GetBytes(value));
    }
}