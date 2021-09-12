using System.Runtime.Serialization;

namespace ExLibris.Core
{
    [DataContract(Name = "excel_value_configuration")]
    public class ExcelValueConfiguration
    {
        [DataMember(Name = "excel_error_to", Order = 10)]
        public ExcelValueTo ExcelErrorTo { get; set; } = ExcelValueTo.Error;

        [DataMember(Name = "excel_missing_to", Order = 20)]
        public ExcelValueTo ExcelMissingTo { get; set; } = ExcelValueTo.Null;

        [DataMember(Name = "excel_empty_to", Order = 30)]
        public ExcelValueTo ExcelEmptyTo { get; set; } = ExcelValueTo.Null;

        [DataMember(Name = "excel_string_empty_to", Order = 40)]
        public ExcelValueTo ExcelStringEmptyTo { get; set; } = ExcelValueTo.Null;

        [DataMember(Name = "null_to_excel", Order = 110)]
        public ToExcelValue NullToExcel { get; set; } = ToExcelValue.ExcelEmpty;

        [DataMember(Name = "string_empty_to_excel", Order = 120)]
        public ToExcelValue StringEmptyToExcel { get; set; } = ToExcelValue.ExcelEmpty;
    }
}
