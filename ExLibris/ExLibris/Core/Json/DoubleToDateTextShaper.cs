using System;
using System.Collections.Generic;

namespace ExLibris.Core.Json
{
    class DoubleToDateTextShaper : IJsonValueShaper
    {
        private string dateFormat;
        private List<string> keys;

        public DoubleToDateTextShaper(string dateFormat, List<string> keys)
        {
            this.dateFormat = dateFormat;
            this.keys = keys;
        }

        public bool ShouldShape(string key, object value) => keys.Contains(key) && value is double;


        public object Shape(object value) => DateTime.FromOADate((double)value).ToString(dateFormat);
    }
}
