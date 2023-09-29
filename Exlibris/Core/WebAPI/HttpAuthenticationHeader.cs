using System.Runtime.Serialization;

namespace Exlibris.Core.WebAPI
{
    [DataContract(Name = "http_authentication_header")]
    public class HttpAuthenticationHeader
    {
        [DataMember(Name = "schema")]
        public string Schema { get; set; }

        [DataMember(Name = "certification")]
        public string Certification { get; set; }

        [DataMember(Name = "basic")]
        public BasicAuthentication Basic { get; set; }

        [DataMember(Name = "bearer")]
        public Bearer Bearer { get; set; }
    }
}