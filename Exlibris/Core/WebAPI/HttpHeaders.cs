using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Exlibris.Core.WebAPI
{
    [DataContract(Name = "http_headers")]
    public class HttpHeaders
    {
        [DataMember(Name = "headers")]
        public List<HttpHeader> Headers { get; set; } = new List<HttpHeader>();

        [DataMember(Name = "authentication")]
        public HttpAuthenticationHeader AuthenticationHeader { get; set; }
    }
}