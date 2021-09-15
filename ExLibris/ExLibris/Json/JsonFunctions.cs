using ExcelDna.Integration;
using ExLibris.Core;
using ExLibris.Core.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ExLibris.Json
{
    public static class JsonFunctions
    {
        [ExcelFunction(
            Name = "ExLibris.Json.CreateJsonObject",
            Category = "ExLibris.Json")]
        public static object CreateJsonObject(object param, string configurationHandle)
        {
            var context = ExLibrisContext.DefaultContext;
            var configuration = context.GetConfiguration(configurationHandle);

            return ObserveJsonObjectHandle(
                nameof(CreateJsonObject),
                context.ObjectRepository,
                () => CreateJsonObject(
                    param, 
                    context, 
                    ExLibrisUtility.GetExcelValueConverter(configuration), 
                    configuration.jsonObjectConfiguration.GetJsonValueConverter()),
                param,
                configurationHandle);
        }

        private static object CreateJsonObject(object param, ExLibrisContext context, ExcelValueConverter valueConverter, JsonValueConverter jsonValueConverter)
        {
            if (param is object[,])
            {
                return CreateJsonObjectByMatrix(valueConverter.GetExcelMatrixAccessor((object[,])param), context, jsonValueConverter);
            }

            var value = valueConverter.ToValue(param);

            if (param is string)
            {
                return CreateJsonObjectByJsonText((string)value, context, jsonValueConverter);
            }

            {
                return new JsonObjectBuilder(context.ObjectRepository, jsonValueConverter)
                            .SetOnlyRootValue(value)
                            .BuildJsonObject();
            }
        }

        internal static object CreateJsonObjectByMatrix(ExcelMatrixAccessor matrix, ExLibrisContext context, JsonValueConverter jsonValueConverter)
        {
            var job = new JsonObjectBuilder(context.ObjectRepository, jsonValueConverter);

            foreach(var row in matrix.Rows)
            {
                var keypath = (string)row[0];
                var value = row[1];

                job.AddJsonValue(keypath, value);
            }

            return job.BuildJsonObject();
        }

        private static object CreateJsonObjectByJsonText(string jsonText, ExLibrisContext context, JsonValueConverter jsonValueConverter)
            => JsonObjectSerialiser.JsonTextToJsonObject(jsonText) ??
                    new JsonObjectBuilder(context.ObjectRepository, jsonValueConverter)
                    .SetOnlyRootValue(jsonText)
                    .BuildJsonObject();

        [ExcelFunction(
             Name = "ExLibris.Json.LoadJsonTextFile",
             Category = "ExLibris.Json")]
        public static object LoadJsonTextFile(string jsonFile, string configurationHandle)
        {
            var context = ExLibrisContext.DefaultContext;
            var configuration = context.GetConfiguration(configurationHandle);

            return ObserveJsonObjectHandle(
                nameof(CreateJsonObject),
                context.ObjectRepository,
                () => CreateJsonObjectByJsonText(
                    File.ReadAllText(jsonFile),
                    context,
                    configuration.jsonObjectConfiguration.GetJsonValueConverter()),
                jsonFile,
                configurationHandle);
        }

        [ExcelFunction(
            Name = "ExLibris.Json.ShowJsonText",
            Category = "ExLibris.Json")]
        public static object ShowJsonText(string objectHandle, bool pretty = false, bool rightNow = false)
        {
            var context = ExLibrisContext.DefaultContext;

            if (rightNow)
            {
                return pretty ?
                        JsonObjectSerialiser.ToJsonPrettyText(context.ObjectRepository.GetObject(objectHandle)) :
                        JsonObjectSerialiser.ToJsonText(context.ObjectRepository.GetObject(objectHandle));

            }
            else
            {
                return ExLibrisUtility.RunAsync(
                    nameof(ShowJsonText),
                    () => pretty ?
                            JsonObjectSerialiser.ToJsonPrettyText(context.ObjectRepository.GetObject(objectHandle)) :
                            JsonObjectSerialiser.ToJsonText(context.ObjectRepository.GetObject(objectHandle)),
                    objectHandle, pretty);
            }
        }

        [ExcelFunction(
            Name = "ExLibris.Json.GetJsonValue",
            Category = "ExLibris.Json")]
        public static object GetJsonValue(string objectHandle, string keyPath, string configurationHandle)
        {
            var context = ExLibrisContext.DefaultContext;
            var configuration = context.GetConfiguration(configurationHandle);

            return ExcelAsyncUtil.Observe(
                nameof(GetJsonValue),
                new object[] { objectHandle, keyPath, configurationHandle, },
                () => ExLibrisUtility.FuncOrObjservableNAIfThrown(() =>
                {
                    var ev = ExLibrisUtility.GetExcelValueConverter(configuration);

                    var jo = context.ObjectRepository.GetObject(objectHandle);
                    var value = new JsonObjectAccessor(jo).GetJsonValue(keyPath);

                    if (JsonUtility.IsJsonDictionaryOrArray(value))
                    {
                        return JsonUtility.NewJsonObjectHandle(context.ObjectRepository, value);
                    }
                    else
                    {
                        return ExLibrisUtility.NewExcelObservableDoNothingOnDisposing(ev.ToExcel(value));
                    }
                })
                );
        }

        [ExcelFunction(
            Name = "ExLibris.Json.GetJsonKeyValues",
            Category = "ExLibris.Json")]
        public static object GetJsonKeyValues(string objectHandle, string configurationHandle)
        {
            var context = ExLibrisContext.DefaultContext;
            var configuration = context.GetConfiguration(configurationHandle);

            return ExLibrisUtility.RunAsync(
                nameof(GetJsonKeyValues),
                () => CreateJsonKeyValueTable(context.ObjectRepository.GetObject(objectHandle), ExLibrisUtility.GetExcelValueConverter(configuration)),
                objectHandle,
                configurationHandle);
        }

        internal static object[,] CreateJsonKeyValueTable(object jo, ExcelValueConverter valueConverter)
        {
            var values = new JsonObjectAccessor(jo).GetJsonValues().ToList();

            var mb = valueConverter.GetExcelMatrixBuilder(values.Count, 2);

            for (var i = 0; i < values.Count; ++i)
            {
                mb[i, 0] = values[i].KeyPath;
                mb[i, 1] = values[i].Value;
            }

            return mb.BuildExcelMatrix();
        }

        [ExcelFunction(
            Name = "ExLibris.Json.CreateJsonArray",
            Category = "ExLibris.Json")]
        public static object CreateJsonArray(object[,] param, string configurationHandle)
        {
            var context = ExLibrisContext.DefaultContext;
            var configuration = context.GetConfiguration(configurationHandle);
            var matrix = ExLibrisUtility.GetExcelValueConverter(configuration).GetExcelMatrixAccessor(param);

            return ObserveJsonObjectHandle(
                nameof(CreateJsonArray),
                context.ObjectRepository,
                () => CreateJsonArray(matrix, context, configuration.jsonObjectConfiguration.GetJsonValueConverter()),
                param,
                configurationHandle);
        }

        private static List<object> CreateJsonArray(ExcelMatrixAccessor param, ExLibrisContext context, JsonValueConverter jsonValueConverter)
        {
            var keys = param.Rows.First().Values.Cast<string>().ToList();

            var jo = new List<object>();

            foreach(var row in param.Rows.Skip(1))
            {
                var job = new JsonObjectBuilder(context.ObjectRepository, jsonValueConverter);
                for (var c = 0; c < row.ColumnSize; ++c)
                {
                    job.AddJsonValue(keys[c], row[c]);
                }
                jo.Add(job.BuildJsonObject());
            }

            return jo;
        }


        [ExcelFunction(
            Name = "ExLibris.Json.GetJsonTable",
            Category = "ExLibris.Json")]
        public static object GetJsonTable(string objectHandle, string configurationHandle)
        {
            var context = ExLibrisContext.DefaultContext;
            var configuration = context.GetConfiguration(configurationHandle);

            var valueConverter = ExLibrisUtility.GetExcelValueConverter(configuration);
;
            return ExLibrisUtility.RunAsync(
                nameof(GetJsonTable),
                () =>
                {
                    var jo = context.ObjectRepository.GetObject(objectHandle);

                    if (JsonUtility.IsJsonDictionary(jo))
                    {
                        return CreateJsonTable(JsonUtility.CastJsonDictionary(jo), valueConverter);
                    }
                    else if (JsonUtility.IsJsonArray(jo))
                    {
                        return CreateJsonTable(JsonUtility.CastJsonArray(jo), valueConverter);
                    }

                    return valueConverter.ToExcel(jo);
                },
                objectHandle,
                configurationHandle);
        }

        private static object[,] CreateJsonTable(Dictionary<string, object> jdictionary, ExcelValueConverter valueConverter)
        {
            var joa = new JsonObjectAccessor(jdictionary);
            var keyPaths = joa.GetJsonValues()
            .Select(kv => kv.KeyPath)
            .ToArray();

            var values = valueConverter.GetExcelMatrixBuilder(2, keyPaths.Length);

            var c = 0;
            foreach (var keyPath in keyPaths)
            {
                values[0, c] = keyPath;
                values[1, c] = joa.GetJsonValue(keyPath);
                ++c;
            }

            return values.BuildExcelMatrix();
        }

        private static object[,] CreateJsonTable(List<object> jarray, ExcelValueConverter valueConverter)
        {
            var joas = jarray
                .Select(joae => new JsonObjectAccessor(joae))
                .ToList();

            var keyPaths = joas
            .SelectMany(joa => joa.GetJsonValues().Select(kv => kv.KeyPath))
            .Distinct()
            .ToList();

            var values = valueConverter.GetExcelMatrixBuilder(joas.Count + 1, keyPaths.Count);
            {
                var c = 0;
                foreach (var keyPath in keyPaths)
                {
                    values[0, c] = keyPath;
                    ++c;
                }
            }

            var r = 1;
            foreach (var joa in joas)
            {
                var c = 0;
                foreach (var keyPath in keyPaths)
                {
                    values[r, c] = joa.GetJsonValue(keyPath);
                    ++c;
                }
                ++r;
            }

            return values.BuildExcelMatrix();
        }

        private static object ObserveJsonObjectHandle(
            string collerFunctionName,
            ObjectRepository objectRepository,
            Func<object> jsonObjectFunc,
            params object[] paramObjects)
            => ExcelAsyncUtil.Observe(
                    collerFunctionName,
                    paramObjects,
                    () => ExLibrisUtility.FuncOrObjservableNAIfThrown(() => JsonUtility.NewJsonObjectHandle(objectRepository, jsonObjectFunc()))
                    );
    }
}
