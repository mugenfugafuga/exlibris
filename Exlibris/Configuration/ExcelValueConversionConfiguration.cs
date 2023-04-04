using System.Runtime.Serialization;

namespace Exlibris.Configuration;

[DataContract(Name = "excel_value_configuration")]
public class ExcelValueConversionConfiguration
{
    [DataMember(Name = "excel_error_to", Order = 10)]
    public ObjectType ExcelErrorTo { get; set; } = ObjectType.ExcelError;

    [DataMember(Name = "excel_missing_to", Order = 20)]
    public ObjectType ExcelMissingTo { get; set; } = ObjectType.Null;

    [DataMember(Name = "excel_empty_to", Order = 30)]
    public ObjectType ExcelEmptyTo { get; set; } = ObjectType.Null;

    [DataMember(Name = "excel_string_empty_to", Order = 40)]
    public ObjectType ExcelStringEmptyTo { get; set; } = ObjectType.Null;

    [DataMember(Name = "null_to", Order = 110)]
    public ExcelValueType NullTo { get; set; } = ExcelValueType.StringEmpty;

    [DataMember(Name = "string_empty_to", Order = 120)]
    public ExcelValueType StringEmptyTo { get; set; } = ExcelValueType.StringEmpty;
}
