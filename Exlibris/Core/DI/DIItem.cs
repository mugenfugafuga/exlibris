using System.Runtime.Serialization;

namespace Exlibris.Core.DI;

[DataContract(Name = "di_item")]
public class DIItem
{
    [DataMember(Name = "implement_type")]
    public string? ImplementType { get; set; }

    [DataMember(Name = "service_type")]
    public string? ServiceType { get; set; }
}
