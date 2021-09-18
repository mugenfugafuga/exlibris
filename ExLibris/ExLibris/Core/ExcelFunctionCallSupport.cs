using ExLibris.Core.Json;
using System;

namespace ExLibris.Core
{
    public class ExcelFunctionCallSupport
    {
        private Lazy<ExcelValueConverter> lazyExcelValueConverter;
        private Lazy<JsonValueConverter> lazyJsonValueConverter;

        public ExcelFunctionCallSupport(ObjectRepository objectRepository, ExLibrisConfiguration configuration)
        {
            ObjectRepository = objectRepository;
            Configuration = configuration;

            lazyExcelValueConverter = new Lazy<ExcelValueConverter>(() => new ExcelValueConverter(Configuration.ExcelValueConfiguration));
            lazyJsonValueConverter = new Lazy<JsonValueConverter>(() => Configuration.JsonObjectConfiguration.GetJsonValueConverter());
        }

        public ObjectRepository ObjectRepository { get; }

        public ExLibrisConfiguration Configuration { get; }

        public ExcelValueConverter GetExcelValueConverter() => lazyExcelValueConverter.Value;

        public JsonValueConverter GetJsonValueConverter()
            => lazyJsonValueConverter.Value;

        public ExcelMatrixAccessor GetExcelMatrixAccessor(object[,] excelMatrix) => new ExcelMatrixAccessor(GetExcelValueConverter(), excelMatrix);

        public ExcelMatrixBuilder GetExcelMatrixBuilder(int rowSize, int columnSize) => new ExcelMatrixBuilder(GetExcelValueConverter(), rowSize, columnSize);

        public JsonObjectAccessor NewJsonObjectAccessor(object jsonObject) => new JsonObjectAccessor(jsonObject);

        public JsonObjectBuilder NewJsonObjectBuilder() => new JsonObjectBuilder(ObjectRepository, GetJsonValueConverter());
    }
}
