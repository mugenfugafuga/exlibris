using System.Runtime.Serialization;

namespace Exlibris.Core
{
    [DataContract(Name = "payload_type")]
    public enum PayloadType
    {
        [EnumMember(Value = "byte")]
        Byte = 0,

        [EnumMember(Value = "string")]
        String = 1,

        [EnumMember(Value = "json")]
        Json = 2,
    }
}
