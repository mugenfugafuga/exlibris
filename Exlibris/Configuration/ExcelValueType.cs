using System.Runtime.Serialization;

namespace Exlibris.Configuration
{
    [DataContract(Name = "excel_value_type")]
    public enum ExcelValueType
    {
        [EnumMember(Value = "excel_empty")]
        ExcelEmpty,

        [EnumMember(Value = "string_empty")]
        StringEmpty,
    }
}

