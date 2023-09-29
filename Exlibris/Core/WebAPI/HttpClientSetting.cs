using System.Runtime.Serialization;

namespace Exlibris.Core.WebAPI
{
    [DataContract(Name = "http_client_setting")]
    public class HttpClientSetting
    {
        [DataMember(Name = "headers")]
        public HttpHeaders Headers { get; set; } = new HttpHeaders();

        [DataMember(Name = "version")]
        public string Version { get; set; }

        [DataMember(Name = "base_address")]
        public string BaseAddress { get; set; }

        [DataMember(Name = "timeout")]
        public string Timeout { get; set; }

        [DataMember(Name = "max_response_buffer_size")]
        public long? MaxResponseContentBufferSize { get; set; }
    }
}