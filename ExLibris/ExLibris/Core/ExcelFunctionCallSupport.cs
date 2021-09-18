using ExLibris.Core.Json;
using System;

namespace ExLibris.Core
{
    public class ExcelFunctionCallSupport
    {
        private Lazy<JsonValueConverter> lazyJsonValueConverter;

        public ExcelFunctionCallSupport(ObjectRepository objectRepository, ExLibrisConfiguration configuration)
        {
            ObjectRepository = objectRepository;
            Configuration = configuration;

            lazyJsonValueConverter = new Lazy<JsonValueConverter>(() => Configuration.JsonObjectConfiguration.GetJsonValueConverter());
        }

        public ObjectRepository ObjectRepository { get; }

        public ExLibrisConfiguration Configuration { get; }

        public ExcelValueConverter GetExcelValueConverter()
            => new ExcelValueConverter(Configuration.ExcelValueConfiguration);

        public JsonValueConverter GetJsonValueConverter()
            => lazyJsonValueConverter.Value;
    }
}
