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
            Name = prefixFunctionName + nameof(CreateJsonObject),
            Category = categoryName)]
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
            if (param is object[,] matrix)
            {
                return support.CreateJsonObject(matrix);
            }

            var value = support.ToValue(param);

            if (param is string stringValue)
            {
                return support.CreateJsonObject(stringValue);
            }

            {
                return support.NewJsonObjectBuilder()
                            .SetOnlyRootValue(value)
                            .BuildJsonObject();
            }
        }

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
                () => support.CreateJsonObject(File.ReadAllText(jsonFile)),
                jsonFile,
                configurationHandle);
        }

        [ExcelFunction(
            Name = prefixFunctionName + nameof(ShowJsonText),
            Category = categoryName)]
        public static object ShowJsonText(string objectHandle, bool pretty = false)
        {
            var context = ExLibrisContext.DefaultContext;
            var support = context.GetFunctionCallSupport();

            return pretty ?
                    JsonObjectSerialiser.ToJsonPrettyText(support.ObjectRepository.GetObject(objectHandle)) :
                    JsonObjectSerialiser.ToJsonText(support.ObjectRepository.GetObject(objectHandle));
        }

        [ExcelFunction(
            Name = prefixFunctionName + nameof(ShowJsonTextAsync),
            Category = categoryName)]
        public static object ShowJsonTextAsync(string objectHandle, bool pretty = false)
        {
            var context = ExLibrisContext.DefaultContext;
            var support = context.GetFunctionCallSupport();

            return ExLibrisUtility.RunAsync(
                nameof(ShowJsonTextAsync),
                () => pretty ?
                        JsonObjectSerialiser.ToJsonPrettyText(support.ObjectRepository.GetObject(objectHandle)) :
                        JsonObjectSerialiser.ToJsonText(support.ObjectRepository.GetObject(objectHandle)),
                objectHandle,
                pretty);
        }

        [ExcelFunction(
            Name = prefixFunctionName + nameof(GetJsonValue),
            Category = categoryName)]
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
                        return JsonUtility.NewObservableJsonObjectHandle(support.ObjectRepository, () => value);
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
                        return ExLibrisUtility.NewObservableObjectHandleAsync(() => support.ToExcel(value));
                    }
                },
                objectHandle,
                keyPath,
                configurationHandle
                );
        }

        [ExcelFunction(
        Name = prefixFunctionName + nameof(GetJsonKeyValues),
        Category = categoryName)]
        public static object GetJsonKeyValues(string objectHandle, object condition, string configurationHandle)
        {
            var context = ExLibrisContext.DefaultContext;
            var support = context.GetFunctionCallSupport(configurationHandle);

            return ExLibrisUtility.FuncOrNAIfThrown(() =>
            {
                if (ExLibrisUtility.IsExcelError(condition))
                {
                    throw new ArgumentException($"{nameof(condition)} is Error.");
                }

                if (ExLibrisUtility.IsExcelMissing(condition) || ExLibrisUtility.IsExcelEmpty(condition))
                {
                    return support.CreateJsonKeyValueMatrix(support.ObjectRepository.GetObject(objectHandle));
                }
                else if (condition is double depth)
                {
                    return ExLibrisUtility.ExcelObserve(
                        nameof(GetJsonKeyValues),
                        () => ExLibrisUtility.NewObservableDisposableObject(
                            () => CreateJsonKeyValueTable(
                                () => support.NewJsonObjectAccessor(support.ObjectRepository.GetObject(objectHandle)).GetJsonValues(Convert.ToInt32(depth)),
                                support)
                            ),
                        objectHandle,
                        configurationHandle,
                        condition);
                }
                else if (condition is string keyPath)
                {
                    return ExLibrisUtility.ExcelObserve(
                        nameof(GetJsonKeyValues),
                        () => ExLibrisUtility.NewObservableDisposableObject(
                            () => CreateJsonKeyValueTable(
                                () => support.NewJsonObjectAccessor(support.ObjectRepository.GetObject(objectHandle)).GetJsonValues(keyPath),
                                support)
                            ),
                        objectHandle,
                        configurationHandle,
                        condition);
                }

                throw new ArgumentException($"{nameof(GetJsonKeyValues)} failed. {nameof(objectHandle)}:{objectHandle}, {nameof(condition)}:{condition}");
            });
        }

        [ExcelFunction(
            Name = prefixFunctionName + nameof(GetJsonKeyValuesAsync),
            Category = categoryName)]
        public static object GetJsonKeyValuesAsync(string objectHandle, object condition, string configurationHandle)
        {
            var context = ExLibrisContext.DefaultContext;
            var support = context.GetFunctionCallSupport(configurationHandle);

            return ExLibrisUtility.FuncOrNAIfThrown(() =>
            {
                if (ExLibrisUtility.IsExcelError(condition))
                {
                    throw new ArgumentException($"{nameof(condition)} is Error.");
                }

                if (ExLibrisUtility.IsExcelMissing(condition) || ExLibrisUtility.IsExcelEmpty(condition))
                {
                    return ExLibrisUtility.RunAsync(
                        nameof(GetJsonKeyValuesAsync),
                        () => support.CreateJsonKeyValueMatrix(support.ObjectRepository.GetObject(objectHandle)),
                        objectHandle,
                        configurationHandle,
                        condition);
                }
                else if (condition is double depth)
                {
                    return ExLibrisUtility.ExcelObserve(
                        nameof(GetJsonKeyValuesAsync),
                        () => ExLibrisUtility.NewObservableDisposableObjectAsync(
                            () => CreateJsonKeyValueTable( 
                                () => support.NewJsonObjectAccessor(support.ObjectRepository.GetObject(objectHandle)).GetJsonValues(Convert.ToInt32(depth)),
                                support)
                            ),
                        objectHandle,
                        configurationHandle,
                        condition);
                }
                else if (condition is string keyPath)
                {
                    return ExLibrisUtility.ExcelObserve(
                        nameof(GetJsonKeyValuesAsync),
                        () => ExLibrisUtility.NewObservableDisposableObjectAsync(
                            () => CreateJsonKeyValueTable(
                                () => support.NewJsonObjectAccessor(support.ObjectRepository.GetObject(objectHandle)).GetJsonValues(keyPath),
                                support)
                            ),
                        objectHandle,
                        configurationHandle,
                        condition);
                }

                throw new ArgumentException($"{nameof(GetJsonKeyValues)} failed. {nameof(objectHandle)}:{objectHandle}, {nameof(condition)}:{condition}");

            });
        }

        private static (object[,] Table, IEnumerable<IDisposable> Disposables) CreateJsonKeyValueTable(
            Func<IEnumerable<(string KeyPath, object Value)>> jsonKeyValueFunc,
            ExcelFunctionCallSupport support)
        {
            var disposables = new List<IDisposable>();
            var values = jsonKeyValueFunc().ToList();

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
            Name = prefixFunctionName + nameof(CreateJsonArray),
            Category = categoryName)]
        public static object CreateJsonArray(object[,] param, string configurationHandle)
        {
            var context = ExLibrisContext.DefaultContext;
            var support = context.GetFunctionCallSupport(configurationHandle);

            return JsonUtility.ObserveJsonObject(
                nameof(CreateJsonArray),
                support.ObjectRepository,
                () => CreateJsonArray(param, support),
                param,
                configurationHandle);
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
            Name = prefixFunctionName + nameof(GetJsonTable),
            Category = categoryName)]
        public static object GetJsonTable(string objectHandle, object[] keys, string configurationHandle)
        {
            var context = ExLibrisContext.DefaultContext;
            var support = context.GetFunctionCallSupport(configurationHandle);

            var jo = support.ObjectRepository.GetObject(objectHandle);

            if (JsonUtility.IsJsonDictionary(jo))
            {
                return CreateJsonTable(JsonUtility.CastJsonDictionary(jo), GetKeyPaths(keys).ToArray(), support);
            }
            else if (JsonUtility.IsJsonArray(jo))
            {
                return CreateJsonTable(JsonUtility.CastJsonArray(jo), GetKeyPaths(keys).ToArray(), support);
            }

            return support.ToExcel(jo);
        }

        [ExcelFunction(
            Name = prefixFunctionName + nameof(GetJsonTableAsync),
            Category = categoryName)]
        public static object GetJsonTableAsync(string objectHandle, object[] keys, string configurationHandle)
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
                        return CreateJsonTable(JsonUtility.CastJsonDictionary(jo), GetKeyPaths(keys).ToArray(), support);
                    }
                    else if (JsonUtility.IsJsonArray(jo))
                    {
                        return CreateJsonTable(JsonUtility.CastJsonArray(jo), GetKeyPaths(keys).ToArray(), support);
                    }

                    return support.ToExcel(jo);
                },
                objectHandle,
                keys,
                configurationHandle);
        }

        private static IEnumerable<string> GetKeyPaths(IEnumerable<object> keys)
        {
            foreach(var key in keys)
            {
                if (ExLibrisUtility.IsExcelError(key))
                {
                    throw new ArgumentException("the error has occurred.");
                }

                if (ExLibrisUtility.IsExcelMissing(key) || ExLibrisUtility.IsExcelMissing(key) || ExLibrisUtility.IsExcelEmpty(key))
                {
                    continue;
                }

                yield return key.ToString();
            }
        }

        private static object[,] CreateJsonTable(Dictionary<string, object> jdictionary, string[] keys, ExcelFunctionCallSupport support)
        {
            var joa = support.NewJsonObjectAccessor(jdictionary);
            var keyPaths = keys.Length == 0 ?
                joa.GetJsonValues()
            .Select(kv => kv.KeyPath)
            .ToArray() :
            keys;

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

        private static object[,] CreateJsonTable(List<object> jarray, string[] keys, ExcelFunctionCallSupport support)
        {
            var joas = jarray
                .Select(joae => support.NewJsonObjectAccessor(joae))
                .ToList();

            var keyPaths = keys.Length == 0 ?
                joas
                .SelectMany(joa => joa.GetJsonValues().Select(kv => kv.KeyPath))
                .Distinct()
                .ToArray() :
                keys;

            var values = support.GetExcelMatrixBuilder(joas.Count + 1, keyPaths.Length);
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
            Name = prefixFunctionName + nameof(SearchJsonArrayElements),
            Category = categoryName)]
        public static object SearchJsonArrayElements(string jsonArrayHandle, string relativeKeyPath, object searchValue, string configurationHandle)
        {
            var context = ExLibrisContext.DefaultContext;
            var support = context.GetFunctionCallSupport(configurationHandle);

            return JsonUtility.ObserveJsonObject(
                nameof(SearchJsonArrayElements),
                support.ObjectRepository,
                () => SearchJsonArrayElements(
                    jsonArrayHandle,
                    relativeKeyPath,
                    searchValue,
                    support),
                jsonArrayHandle,
                relativeKeyPath,
                searchValue,
                configurationHandle);
        }

        [ExcelFunction(
            Name = prefixFunctionName + nameof(SearchJsonArrayElementsAsync),
            Category = categoryName)]
        public static object SearchJsonArrayElementsAsync(string jsonArrayHandle, string relativeKeyPath, object searchValue, string configurationHandle)
        {
            var context = ExLibrisContext.DefaultContext;
            var support = context.GetFunctionCallSupport(configurationHandle);

            return JsonUtility.ObserveJsonObjectAsync(
                nameof(CreateJsonObjectAsync),
                support.ObjectRepository,
                () => SearchJsonArrayElements(
                    jsonArrayHandle,
                    relativeKeyPath,
                    searchValue,
                    support),
                jsonArrayHandle,
                relativeKeyPath, 
                searchValue,
                configurationHandle);
        }

        private static object SearchJsonArrayElements(string jsonArrayHandle, string relativeKeyPath, object searchValue, ExcelFunctionCallSupport support)
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