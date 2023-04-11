using System.Runtime.Serialization;

namespace Exlibris.Core.WebAPI;

[DataContract(Name = "http_request_setting")]
public class HttpRequestSetting
{
    [DataMember(Name = "headers")]
    public HttpHeaders Headers { get; set; } = new HttpHeaders();

    [DataMember(Name = "version")]
    public string? Version { get; set; }

    [DataMember(Name = "version_policy")]
    public HttpVersionPolicy? VersionPolicy { get; set; }
}
