using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ExLibris.Core.Json
{
    [DataContract(Name = "json_object_configuration")]
    public class JsonObjectConfiguration
    {
        [DataMember(Name = "double_to_date_text", Order = 120)]
        public List<DoubleToDateTextSetting> DoubleToDateTextSettings { get; set; }

        public JsonValueConverter GetJsonValueConverter() => new JsonValueConverter(GetValueShapers());

        private IEnumerable<IJsonValueShaper> GetValueShapers()
        {
            foreach(var setting in DoubleToDateTextSettings)
            {
                yield return new DoubleToDateTextShaper(setting.DateFormat, setting.TargetKeys);
            }
        }
    }

    [CollectionDataContract(Name = "double_to_date_text")]
    public class DoubleToDateTextSetting
    {
        [DataMember(Name = "date_format", Order = 10)]
        public string DateFormat { get; set; }
        
        [DataMember(Name = "keys", Order = 20)]
        public List<string> TargetKeys { get; set; }
    }
}
