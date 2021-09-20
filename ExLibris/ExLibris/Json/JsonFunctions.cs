using ExcelDna.Integration;
using ExLibris.Core;
using ExLibris.Core.Json;
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
            var support = context.GetFunctionCallSupport(configurationHandle);

            return JsonUtility.ObserveJsonObject(
                nameof(CreateJsonObject),
                support.ObjectRepository,
                () => CreateJsonObject(
                    param, 
                    support),
                param,
                configurationHandle);
        }

        private static object CreateJsonObject(object param, ExcelFunctionCallSupport support)
        {
            if (param is object[,])
            {
                return CreateJsonObjectByMatrix(support.GetExcelMatrixAccessor((object[,])param), support);
            }

            var value = support.ToValue(param);

            if (param is string)
            {
                return CreateJsonObjectByJsonText((string)value, support);
            }

            {
                return support.NewJsonObjectBuilder()
                            .SetOnlyRootValue(value)
                            .BuildJsonObject();
            }
        }

        internal static object CreateJsonObjectByMatrix(ExcelMatrixAccessor matrix, ExcelFunctionCallSupport support)
        {
            var job = support.NewJsonObjectBuilder();

            foreach(var row in matrix.Rows)
            {
                var keypath = (string)row[0];
                var value = row[1];

                job.AddJsonValue(keypath, value);
            }

            return job.BuildJsonObject();
        }

        private static object CreateJsonObjectByJsonText(string jsonText, ExcelFunctionCallSupport support)
            => JsonObjectSerialiser.JsonTextToJsonObject(jsonText) ??
                    support.NewJsonObjectBuilder()
                    .SetOnlyRootValue(jsonText)
                    .BuildJsonObject();

        [ExcelFunction(
             Name = "ExLibris.Json.LoadJsonTextFile",
             Category = "ExLibris.Json")]
        public static object LoadJsonTextFile(string jsonFile, string configurationHandle)
        {
            var context = ExLibrisContext.DefaultContext;
            var support = context.GetFunctionCallSupport(configurationHandle);

            return JsonUtility.ObserveJsonObject(
                nameof(CreateJsonObject),
                support.ObjectRepository,
                () => CreateJsonObjectByJsonText(
                    File.ReadAllText(jsonFile),
                    support),
                jsonFile,
                configurationHandle);
        }

        [ExcelFunction(
            Name = "ExLibris.Json.ShowJsonText",
            Category = "ExLibris.Json")]
        public static object ShowJsonText(string objectHandle, bool pretty = false, bool rightNow = false)
        {
            var context = ExLibrisContext.DefaultContext;
            var support = context.GetFunctionCallSupport();

            if (rightNow)
            {
                return pretty ?
                        JsonObjectSerialiser.ToJsonPrettyText(support.ObjectRepository.GetObject(objectHandle)) :
                        JsonObjectSerialiser.ToJsonText(support.ObjectRepository.GetObject(objectHandle));

            }
            else
            {
                return ExLibrisUtility.RunAsync(
                    nameof(ShowJsonText),
                    () => pretty ?
                            JsonObjectSerialiser.ToJsonPrettyText(support.ObjectRepository.GetObject(objectHandle)) :
                            JsonObjectSerialiser.ToJsonText(support.ObjectRepository.GetObject(objectHandle)),
                    objectHandle, pretty);
            }
        }

        [ExcelFunction(
            Name = "ExLibris.Json.GetJsonValue",
            Category = "ExLibris.Json")]
        public static object GetJsonValue(string objectHandle, string keyPath, string configurationHandle)
        {
            var context = ExLibrisContext.DefaultContext;
            var support = context.GetFunctionCallSupport(configurationHandle);

            return ExLibrisUtility.ExcelObserve(
                nameof(GetJsonValue),
                () => 
                {
                    var jo = support.ObjectRepository.GetObject(objectHandle);
                    var value = support.NewJsonObjectAccessor(jo).GetJsonValue(keyPath);

                    if (JsonUtility.IsJsonDictionaryOrArray(value))
                    {
                        return JsonUtility.NewJsonObjectHandle(support.ObjectRepository, () => value);
                    }
                    else
                    {
                        return ExLibrisUtility.NewObservableObjectHandle(() => support.ToExcel(value));
                    }
                },
                objectHandle,
                keyPath,configurationHandle
                );
        }

        [ExcelFunction(
            Name = "ExLibris.Json.GetJsonKeyValues",
            Category = "ExLibris.Json")]
        public static object GetJsonKeyValues(string objectHandle, string configurationHandle)
        {
            var context = ExLibrisContext.DefaultContext;
            var support = context.GetFunctionCallSupport(configurationHandle);

            return ExLibrisUtility.RunAsync(
                nameof(GetJsonKeyValues),
                () => CreateJsonKeyValueTable(support.ObjectRepository.GetObject(objectHandle), support),
                objectHandle,
                configurationHandle);
        }

        internal static object[,] CreateJsonKeyValueTable(object jo, ExcelFunctionCallSupport support)
        {
            var values = support.NewJsonObjectAccessor(jo).GetJsonValues().ToList();

            var mb = support.GetExcelMatrixBuilder(values.Count, 2);

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
            var support = context.GetFunctionCallSupport(configurationHandle);

            var matrix = support.GetExcelMatrixAccessor(param);

            return JsonUtility.ObserveJsonObject(
                nameof(CreateJsonArray),
                support.ObjectRepository,
                () => CreateJsonArray(matrix, support),
                param,
                configurationHandle);
        }

        private static List<object> CreateJsonArray(ExcelMatrixAccessor param, ExcelFunctionCallSupport support)
        {
            var keys = param.Rows.First().Values.Cast<string>().ToList();

            var jo = new List<object>();

            foreach(var row in param.Rows.Skip(1))
            {
                var job = support.NewJsonObjectBuilder();
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
            var support = context.GetFunctionCallSupport(configurationHandle);

            return ExLibrisUtility.RunAsync(
                nameof(GetJsonTable),
                () =>
                {
                    var jo = support.ObjectRepository.GetObject(objectHandle);

                    if (JsonUtility.IsJsonDictionary(jo))
                    {
                        return CreateJsonTable(JsonUtility.CastJsonDictionary(jo), support);
                    }
                    else if (JsonUtility.IsJsonArray(jo))
                    {
                        return CreateJsonTable(JsonUtility.CastJsonArray(jo), support);
                    }

                    return support.ToExcel(jo);
                },
                objectHandle,
                configurationHandle);
        }

        private static object[,] CreateJsonTable(Dictionary<string, object> jdictionary, ExcelFunctionCallSupport support)
        {
            var joa = support.NewJsonObjectAccessor(jdictionary);
            var keyPaths = joa.GetJsonValues()
            .Select(kv => kv.KeyPath)
            .ToArray();

            var values = support.GetExcelMatrixBuilder(2, keyPaths.Length);

            var c = 0;
            foreach (var keyPath in keyPaths)
            {
                values[0, c] = keyPath;
                values[1, c] = joa.GetJsonValue(keyPath);
                ++c;
            }

            return values.BuildExcelMatrix();
        }

        private static object[,] CreateJsonTable(List<object> jarray, ExcelFunctionCallSupport support)
        {
            var joas = jarray
                .Select(joae => support.NewJsonObjectAccessor(joae))
                .ToList();

            var keyPaths = joas
            .SelectMany(joa => joa.GetJsonValues().Select(kv => kv.KeyPath))
            .Distinct()
            .ToList();

            var values = support.GetExcelMatrixBuilder(joas.Count + 1, keyPaths.Count);
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
    }
}
