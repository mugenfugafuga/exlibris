using ExLibris.Core.Json;
using System.Runtime.Serialization;

namespace ExLibris.Core
{
    [DataContract(Name = "exlibris_configuration")]
    public class ExLibrisConfiguration
    {
        [DataMember(Name = "excel_value", Order = 10)]
        public ExcelValueConfiguration ExcelValueConfiguration { get; set; } = new ExcelValueConfiguration();

        [DataMember(Name = "json", Order = 110)]
        public JsonObjectConfiguration JsonObjectConfiguration { get; set; } = new Json.JsonObjectConfiguration();
    }
}
