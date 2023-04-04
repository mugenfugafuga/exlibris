using System.Runtime.Serialization;

namespace Exlibris.Configuration;

[DataContract(Name = "exlibris_configuraiton")]
public class ExlibrisConfiguration
{
    [DataMember(Name = "excel_value_conversion", Order = 10)]
    public ExcelValueConversionConfiguration ExcelValueConversion { get; set; } = new ExcelValueConversionConfiguration();
}
