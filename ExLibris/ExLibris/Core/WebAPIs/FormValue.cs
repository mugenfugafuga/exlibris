using System.Runtime.Serialization;

namespace ExLibris.Core.WebAPIs
{
    [DataContract(Name = "form_value")]
    public class FormValue
    {
        [DataMember(Name = "name", Order = 10)]
        public string Name { get; set; }

        [DataMember(Name = "value", Order = 20)]
        public string Value { get; set; }
    }
}
