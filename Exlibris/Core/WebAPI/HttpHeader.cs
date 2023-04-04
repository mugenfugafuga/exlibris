using System.Runtime.Serialization;

namespace Exlibris.Core.WebAPI;

[DataContract(Name = "http_header")]
public class HttpHeader
{
    [DataMember(Name = "key")]
    public string? Key { get; set; }

    [DataMember(Name = "values")]
    public List<string> Values { get; set; } = new List<string>();
}
