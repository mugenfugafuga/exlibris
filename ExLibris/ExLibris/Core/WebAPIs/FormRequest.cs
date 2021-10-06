using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;

namespace ExLibris.Core.WebAPIs
{
    [DataContract(Name = "form_request")]
    public class FormRequest
    {
        [DataMember(Name = "values", Order = 10)]
        public List<FormValue> Values { get; set; } = new List<FormValue>();

        public FormUrlEncodedContent GetHttpContent()
            => new FormUrlEncodedContent(GetDictionary());

        private Dictionary<string, string> GetDictionary()
            => Values?.ToDictionary(fv => fv.Name, fv => fv.Value) ?? new Dictionary<string, string>();
    }
}
