using System.Runtime.Serialization;

namespace Exlibris.Core.WebAPI;

[DataContract(Name = "basic_authentication")]
public class BasicAuthentication
{
    [DataMember(Name = "user")]
    public string? User { get; set; }

    [DataMember(Name = "password")]
    public string? Password { get; set; }

    [DataMember(Name = "base64")]
    public string? Base64 { get; set; }
}
