using ExLibris.Core;
using ExLibris.Core.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ExLibris
{
    class ExcelFunctionCallSupport
    {
        private readonly Lazy<ExcelValueConverter> lazyExcelValueConverter;
        private readonly Lazy<JsonValueConverter> lazyJsonValueConverter;

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

        public object ToValue(object excelValue) => GetExcelValueConverter().ToValue(excelValue);

        public object ToExcel(object value) => GetExcelValueConverter().ToExcel(value);

        public string ToValueAsString(object excelValue) => GetExcelValueConverter().ToValue(excelValue) as string;

        public object CreateJsonObject(object[,] excelMatrix)
        {
            var matrix = GetExcelMatrixAccessor(excelMatrix);
            var job = NewJsonObjectBuilder();

            foreach (var row in matrix.Rows)
            {
                var keypath = (string)row[0];
                var value = row[1];

                job.AddJsonValue(keypath, value);
            }

            return job.BuildJsonObject();
        }

        public object CreateJsonObject(string jsonText)
            => JsonUtility.CreateJsonObjectFromJsonText(jsonText, ObjectRepository, GetJsonValueConverter());

        public T CreateObject<T>(object[,] excelMatrix)
            => JsonObjectSerialiser.ToObject<T>(CreateJsonObject(excelMatrix));

        public object[,] CreateJsonKeyValueMatrix(object jsonObject)
        {
            var jo = JsonUtility.IsJsonDictionaryOrArray(jsonObject) ?
                jsonObject :
                JsonObjectSerialiser.ToJsonObject(jsonObject);

            return CreateJsonKeyValueMatrixFromJsonObject(jo);
        }


        private object[,] CreateJsonKeyValueMatrixFromJsonObject(object jsonObject)
        {
            var values = NewJsonObjectAccessor(jsonObject).GetJsonValues().ToList();

            var mb = GetExcelMatrixBuilder(values.Count, 2);

            for (var i = 0; i < values.Count; ++i)
            {
                mb[i, 0] = values[i].KeyPath;
                mb[i, 1] = values[i].Value;
            }

            return mb.BuildExcelMatrix();
        }

        public IEnumerable<(string Key, string Value)> ConvertKeyValues(object parameters)
        {
            var param = ToValue(parameters);

            if (param != null)
            {

                var jo = param is object[,] matrix ?
                    CreateJsonObject(matrix) :
                    ObjectRepository.GetObject((string)param);

                var kvs = NewJsonObjectAccessor(jo)
                    .GetJsonValues()
                    .Select(kv => (kv.KeyPath ?? string.Empty, kv.Value?.ToString() ?? string.Empty));

                foreach (var kv in kvs)
                {
                    yield return kv;
                }
            }
        }
    }
}
