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
        private const string categoryName = "ExLibris.Json";
        private const string prefixFunctionName = categoryName + ".";

        [ExcelFunction(
            Name = prefixFunctionName + nameof(CreateJsonObjectAsync),
            Category = categoryName)]
        public static object CreateJsonObjectAsync(object param, string configurationHandle)
        {
            var context = ExLibrisContext.DefaultContext;
            var support = context.GetFunctionCallSupport(configurationHandle);

            return JsonUtility.ObserveJsonObjectAsync(
                nameof(CreateJsonObjectAsync),
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

            foreach (var row in matrix.Rows)
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
            Name = prefixFunctionName + nameof(LoadJsonTextFileAsync),
            Category = categoryName)]
        public static object LoadJsonTextFileAsync(string jsonFile, string configurationHandle)
        {
            var context = ExLibrisContext.DefaultContext;
            var support = context.GetFunctionCallSupport(configurationHandle);

            return JsonUtility.ObserveJsonObjectAsync(
                nameof(CreateJsonObjectAsync),
                support.ObjectRepository,
                () => CreateJsonObjectByJsonText(
                    File.ReadAllText(jsonFile),
                    support),
                jsonFile,
                configurationHandle);
        }

        [ExcelFunction(
            Name = prefixFunctionName + nameof(ShowJsonText),
            Category = categoryName)]
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
            Name = prefixFunctionName + nameof(GetJsonValueAsync),
            Category = categoryName)]
        public static object GetJsonValueAsync(string objectHandle, string keyPath, string configurationHandle)
        {
            var context = ExLibrisContext.DefaultContext;
            var support = context.GetFunctionCallSupport(configurationHandle);

            return ExLibrisUtility.ExcelObserve(
                nameof(GetJsonValueAsync),
                () =>
                {
                    var jo = support.ObjectRepository.GetObject(objectHandle);
                    var value = support.NewJsonObjectAccessor(jo).GetJsonValue(keyPath);

                    if (JsonUtility.IsJsonDictionaryOrArray(value))
                    {
                        return JsonUtility.NewObservableJsonObjectHandleAsync(support.ObjectRepository, () => value);
                    }
                    else
                    {
                        return ExLibrisUtility.NewObservableObjectHandle(() => support.ToExcel(value));
                    }
                },
                objectHandle,
                keyPath,
                configurationHandle
                );
        }

        [ExcelFunction(
            Name = prefixFunctionName + nameof(GetJsonKeyValuesAsync),
            Category = categoryName)]
        public static object GetJsonKeyValuesAsync(string objectHandle, string configurationHandle, object depth)
        {
            var context = ExLibrisContext.DefaultContext;
            var support = context.GetFunctionCallSupport(configurationHandle);

            return ExLibrisUtility.FuncOrNAIfThrown(() =>
            {
                if (ExLibrisUtility.IsExcelError(depth))
                {
                    throw new ArgumentException($"{nameof(depth)} is Error.");
                }

                if (ExLibrisUtility.IsExcelMissing(depth) || ExLibrisUtility.IsExcelEmpty(depth))
                {
                    return ExLibrisUtility.RunAsync(
                        nameof(GetJsonKeyValuesAsync),
                        () => CreateJsonKeyValueTable(support.ObjectRepository.GetObject(objectHandle), support),
                        objectHandle,
                        configurationHandle,
                        depth);
                }
                else
                {
                    return ExLibrisUtility.ExcelObserve(
                        nameof(GetJsonKeyValuesAsync),
                        () => ExLibrisUtility.NewObservableDisposableObjectAsync(
                            () => CreateJsonKeyValueTable(support.ObjectRepository.GetObject(objectHandle), support, Convert.ToInt32(depth))),
                        objectHandle,
                        configurationHandle,
                        depth);
                }

            });
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

        internal static (object[,] Table, IEnumerable<IDisposable> Disposables) CreateJsonKeyValueTable(object jo, ExcelFunctionCallSupport support, int depth)
        {
            var disposables = new List<IDisposable>();
            var values = support.NewJsonObjectAccessor(jo).GetJsonValues(depth).ToList();

            var mb = support.GetExcelMatrixBuilder(values.Count, 2);

            for (var i = 0; i < values.Count; ++i)
            {
                mb[i, 0] = values[i].KeyPath;

                var v = values[i].Value;
                if (JsonUtility.IsJsonDictionaryOrArray(v))
                {
                    var vv = JsonUtility.NewJsonObjectHandle(support.ObjectRepository, () => v);
                    vv.CallRegistration();
                    disposables.Add(vv);
                    mb[i, 1] = vv.HandleKey;
                }
                else
                {
                    mb[i, 1] = values[i].Value;
                }
            }

            return (mb.BuildExcelMatrix(), disposables);
        }

        [ExcelFunction(
            Name = prefixFunctionName + nameof(CreateJsonArrayAsync),
            Category = categoryName)]
        public static object CreateJsonArrayAsync(object[,] param, string configurationHandle)
        {
            var context = ExLibrisContext.DefaultContext;
            var support = context.GetFunctionCallSupport(configurationHandle);

            return JsonUtility.ObserveJsonObjectAsync(
                nameof(CreateJsonArrayAsync),
                support.ObjectRepository,
                () => CreateJsonArray(param, support),
                param,
                configurationHandle);
        }

        private static List<object> CreateJsonArray(object[,] param, ExcelFunctionCallSupport support)
        {
            var matrix = support.GetExcelMatrixAccessor(param);

            var keys = matrix.Rows.First().Values.Cast<string>().ToList();

            var jo = new List<object>();

            foreach (var row in matrix.Rows.Skip(1))
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
            Name = prefixFunctionName + nameof(GetJsonTableAsync),
            Category = categoryName)]
        public static object GetJsonTableAsync(string objectHandle, string configurationHandle)
        {
            var context = ExLibrisContext.DefaultContext;
            var support = context.GetFunctionCallSupport(configurationHandle);

            return ExLibrisUtility.RunAsync(
                nameof(GetJsonTableAsync),
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

        [ExcelFunction(
            Name = prefixFunctionName + nameof(SeachJsonArrayElementsAsync),
            Category = categoryName)]
        public static object SeachJsonArrayElementsAsync(string jsonArrayHandle, string relativeKeyPath, object searchValue, string configurationHandle)
        {
            var context = ExLibrisContext.DefaultContext;
            var support = context.GetFunctionCallSupport(configurationHandle);

            return JsonUtility.ObserveJsonObjectAsync(
                nameof(CreateJsonObjectAsync),
                support.ObjectRepository,
                () => SeachJsonArrayElementsAsync(
                    jsonArrayHandle,
                    relativeKeyPath,
                    searchValue,
                    support),
                jsonArrayHandle,
                relativeKeyPath, 
                searchValue,
                configurationHandle);
        }

        public static object SeachJsonArrayElementsAsync(string jsonArrayHandle, string relativeKeyPath, object searchValue, ExcelFunctionCallSupport support)
        {
            var jo = support.ObjectRepository.GetObject(jsonArrayHandle);

            if (!JsonUtility.IsJsonArray(jo))
            {
                return support.ToExcel(null);
            }

            var ja = JsonUtility.CastJsonArray(jo);

            var sjaes = ja
                .Where(jae => support.NewJsonObjectAccessor(jae).GetJsonValue(relativeKeyPath)?.Equals(searchValue) ?? false)
                .ToList();

            if (sjaes.Count > 1)
            {
                return sjaes;
            }
            else if(sjaes.Count == 1)
            {
                var v = sjaes.First();

                return v;
            }
            else
            {
                return support.ToExcel(null);
            }
        }
    }
}