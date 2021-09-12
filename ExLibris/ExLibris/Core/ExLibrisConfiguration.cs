using System.Runtime.Serialization;

namespace ExLibris.Core
{
    [DataContract(Name = "exlibris_configuration")]
    public class ExLibrisConfiguration
    {
        [DataMember(Name = "excel_value", Order = 10)]
        public ExcelValueConfiguration ExcelValueConfiguration { get; set; } = new ExcelValueConfiguration();
    }
}
