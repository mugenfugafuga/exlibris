using System.Runtime.Serialization;

namespace Exlibris.Configuration
{
    [DataContract(Name = "object_type")]
    public enum ObjectType
    {
        [EnumMember(Value = "excel_error")]
        ExcelError,

        [EnumMember(Value = "null")]
        Null,

        [EnumMember(Value = "string_empty")]
        StringEmpty,
    }
}