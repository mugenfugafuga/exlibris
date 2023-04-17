using Exlibris.Core.DI;
using System.Runtime.Serialization;

namespace Exlibris.Configuration;

[DataContract(Name = "exlibris_configuraiton")]
public class ExlibrisConfiguration
{
    [DataMember(Name = "excel_value_conversion", Order = 10)]
    public ExcelValueConversionConfiguration ExcelValueConversion { get; set; } = new ExcelValueConversionConfiguration();

    [DataMember(Name = "di_configuration", Order = 20)]
    public DIConfiguration DIConfiguration { get; set; } = new DIConfiguration();
}
