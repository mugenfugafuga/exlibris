using System.Runtime.Serialization;

namespace Exlibris.Core.WebAPI;

[DataContract(Name = "dearer")]
public class Bearer
{
    [DataMember(Name = "access_token")]
    public string? AccessToken { get; set; }
}
