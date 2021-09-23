using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ExLibris.Core.WebAPIs
{
    [DataContract(Name = "web_api_headers")]
    public class WebAPIHeaders
    {
        [DataMember(Name = "user_agent", Order = 10)]
        public string UserAgent { get; set; }

        [DataMember(Name = "authorization", Order = 20)]
        public Authorization Authorization { get; set; }

        [DataMember(Name = "custom_headers", Order = 110)]
        public List<CustomHeader> CustomHeaders { get; set; } = new List<CustomHeader>();
    }

    [DataContract(Name = "authorization")]
    public class Authorization
    {
        [DataMember(Name = "scheme", Order = 10)]
        public string Scheme { get; set; }

        [DataMember(Name = "parameter", Order = 20)]
        public string Parameter { get; set; }
    }

    [DataContract(Name = "custom_header")]
    public class CustomHeader
    {
        [DataMember(Name = "name", Order = 10)]
        public string Name { get; set; }

        [DataMember(Name = "value", Order = 20)]
        public string Value { get; set; }
    }
}
