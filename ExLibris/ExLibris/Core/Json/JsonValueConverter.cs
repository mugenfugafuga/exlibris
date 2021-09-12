using System.Collections.Generic;
using System.Linq;

namespace ExLibris.Core.Json
{
    public class JsonValueConverter
    {
        private List<IJsonValueShaper> shapers;

        public JsonValueConverter(IEnumerable<IJsonValueShaper> shapers)
        {
            this.shapers = shapers.ToList();
        }

        public object Convert(string key, object value)
        {
            var shaper = shapers.FirstOrDefault(s => s.ShouldShape(key, value));

            return shaper?.Shape(value) ?? value;
        }
    }
}
