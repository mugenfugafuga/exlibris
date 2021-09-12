using ExcelDna.Integration;
using ExLibris.Core;
using ExLibris.Core.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ExLibris.Json
{
    public static class JsonFunctions
    {
        [ExcelFunction(
            Name = "ExLibris.Json.CreateJsonObject",
            Category = "ExLibris.Json"
            )]
        public static object CreateJsonObject(object param)
            => ExcelDnaUtility.FuncOrNAIfThrown(() => CreateJsonObject(param, ExLibrisContext.DefaultContext));

        private static object CreateJsonObject(object param, ExLibrisContext context)
        {
            ExcelDnaUtility.ThrowIfMissingOrError(param, nameof(param));

            if (param is object[,])
            {
                return ObserveJsonObjectHandle(
                    nameof(CreateJsonObject),
                    context.ObjectRepository,
                    () => CreateJsonObjectByMatrix((object[,])param, context),
                    param);
            }

            if (param is string)
            {
                return ObserveJsonObjectHandle(
                    nameof(CreateJsonObject),
                    context.ObjectRepository,
                    () => CreateJsonObjectByJsonText((string)param, context),
                    param);
            }

            {
                return ObserveJsonObjectHandle(
                    nameof(CreateJsonObject),
                    context.ObjectRepository,
                    () => new JsonObjectBuilder(context.ObjectRepository)
                            .SetOnlyRootValue(ExcelDnaUtility.NullIfEmpty(param))
                            .BuildJsonObject(),
                    param);
            }
        }

        private static object CreateJsonObjectByMatrix(object[,] matrix, ExLibrisContext context)
        {
            var rsize = matrix.GetLength(0);

            var job = new JsonObjectBuilder(context.ObjectRepository);

            for (var r = 0; r < rsize; ++r)
            {
                ExcelDnaUtility.ThrowIfMissingOrErrorOrEmpty(matrix[r, 0], () => $"matrix[{r}][0]");
                ExcelDnaUtility.ThrowIfMissingOrError(matrix[r, 1], () => $"matrix[{r}][1]");

                var keypath = (string)ExcelDnaUtility.NullIfEmpty(matrix[r, 0]);
                var value = ExcelDnaUtility.NullIfEmpty(matrix[r, 1]);

                ExcelDnaUtility.ThrowIfMissingOrError(value, () => $"matrix[{r}][1]");

                job.AddJsonValue(keypath, value);
            }

            return job.BuildJsonObject();
        }

        private static object CreateJsonObjectByJsonText(string jsonText, ExLibrisContext context)
            => JsonObjectSerialiser.ToJsonObject(jsonText) ??
                    new JsonObjectBuilder(context.ObjectRepository)
                    .SetOnlyRootValue(jsonText)
                    .BuildJsonObject();

        [ExcelFunction(
            Name = "ExLibris.Json.ShowJsonText",
            Category = "ExLibris.Json"
            )]
        public static object ShowJsonText(string objectHandle, bool pretty)
        {
            var context = ExLibrisContext.DefaultContext;

            return ExcelDnaUtility.ObserveExcelObservableSimply(
                nameof(ShowJsonText),
                () => pretty ?
                        JsonObjectSerialiser.ToJsonPrettyText(context.ObjectRepository.GetObject(objectHandle)) :
                        JsonObjectSerialiser.ToJsonText(context.ObjectRepository.GetObject(objectHandle)),
                objectHandle, pretty);
        }

        [ExcelFunction(
            Name = "ExLibris.Json.GetJsonValue",
            Category = "ExLibris.Json"
            )]
        public static object GetJsonValue(string objectHandle, string keyPath)
        {
            var context = ExLibrisContext.DefaultContext;

            return ExcelAsyncUtil.Observe(
                nameof(GetJsonValue),
                new object[] { objectHandle, keyPath, },
                () => ExcelDnaUtility.FuncOrNAIfThrown(() =>
                {
                    var jo = context.ObjectRepository.GetObject(objectHandle);
                    var value = new JsonObjectAccessor(jo).GetJsonValue(keyPath);

                    if (JsonUtility.IsJsonDictionaryOrArray(value))
                    {
                        return JsonUtility.NewJsonObjectHandle(context.ObjectRepository, value);
                    }
                    else
                    {
                        return ExcelDnaUtility.NewSimpleExcelObservable(ExcelDnaUtility.ToExcelValue(value));
                    }
                })
                );
        }

        [ExcelFunction(
            Name = "ExLibris.Json.GetJsonKeyValues",
            Category = "ExLibris.Json"
            )]
        public static object GetJsonKeyValues(string objectHandle)
        {
            var context = ExLibrisContext.DefaultContext;

            return ExcelDnaUtility.ObserveExcelObservableSimply(
                nameof(GetJsonKeyValues),
                () =>
                {
                    var jo = context.ObjectRepository.GetObject(objectHandle);
                    var values = new JsonObjectAccessor(jo).GetJsonValues().ToList();

                    var excelvalues = new object[values.Count, 2];

                    for (var i = 0; i < values.Count; ++i)
                    {
                        excelvalues[i, 0] = values[i].KeyPath;
                        excelvalues[i, 1] = ExcelDnaUtility.ToExcelValue(values[i].Value);
                    }

                    return excelvalues;
                },
                objectHandle);
        }

        [ExcelFunction(
            Name = "ExLibris.Json.CreateJsonArray",
            Category = "ExLibris.Json"
            )]
        public static object CreateJsonArray(object[,] param)
        {
            var context = ExLibrisContext.DefaultContext;

            return ObserveJsonObjectHandle(
                nameof(CreateJsonArray),
                context.ObjectRepository,
                () => {
                    var rsize = param.GetLength(0);
                    var csize = param.GetLength(1);

                    var keys = Enumerable.Range(0, csize)
                    .Select(i => (string)ExcelDnaUtility.NullIfEmpty(param[0, i]))
                    .ToArray();

                    var jo = new List<object>();

                    for (var r = 1; r < rsize; ++r)
                    {
                        var job = new JsonObjectBuilder(context.ObjectRepository);
                        for (var c = 0; c < csize; ++c)
                        {
                            job.AddJsonValue(keys[c], ExcelDnaUtility.NullIfEmpty(param[r, c]));
                        }
                        jo.Add(job.BuildJsonObject());
                    }

                    return jo;
                },
                param);
        }

        [ExcelFunction(
            Name = "ExLibris.Json.GetJsonTable",
            Category = "ExLibris.Json"
            )]
        public static object GetJsonTable(string objectHandle)
        {
            var context = ExLibrisContext.DefaultContext;

            return ExcelDnaUtility.ObserveExcelObservableSimply(
                nameof(GetJsonTable),
                () =>
                {
                    var jo = context.ObjectRepository.GetObject(objectHandle);

                    if (JsonUtility.IsJsonDictionary(jo))
                    {
                        return CreateJsonTable(JsonUtility.CastJsonDictionary(jo));
                    }
                    else if (JsonUtility.IsJsonArray(jo))
                    {
                        return CreateJsonTable(JsonUtility.CastJsonArray(jo));
                    }

                    return ExcelDnaUtility.ToExcelValue(jo);
                },
                objectHandle);
        }

        private static object CreateJsonTable(Dictionary<string, object> jdictionary)
        {
            var joa = new JsonObjectAccessor(jdictionary);
            var keyPaths = joa.GetJsonValues()
            .Select(kv => kv.KeyPath)
            .ToArray();

            var values = new object[2, keyPaths.Length];

            var c = 0;
            foreach (var keyPath in keyPaths)
            {
                values[0, c] = keyPath;
                values[1, c] = ExcelDnaUtility.ToExcelValue(joa.GetJsonValue(keyPath));
                ++c;
            }

            return values;
        }

        private static object CreateJsonTable(List<object> jarray)
        {
            var joas = jarray
                .Select(joae => new JsonObjectAccessor(joae))
                .ToList();

            var keyPaths = joas
            .SelectMany(joa => joa.GetJsonValues().Select(kv => kv.KeyPath))
            .Distinct()
            .ToList();

            var values = new object[joas.Count + 1, keyPaths.Count];
            {
                var c = 0;
                foreach (var keyPath in keyPaths)
                {
                    values[0, c] = ExcelDnaUtility.ToExcelValue(keyPath);
                    ++c;
                }
            }

            var r = 1;
            foreach (var joa in joas)
            {
                var c = 0;
                foreach (var keyPath in keyPaths)
                {
                    values[r, c] = ExcelDnaUtility.ToExcelValue(joa.GetJsonValue(keyPath));
                    ++c;
                }
                ++r;
            }

            return values;
        }

        private static object ObserveJsonObjectHandle(
            string collerFunctionName,
            ObjectRepository objectRepository,
            Func<object> jsonObjectFunc,
            params object[] paramObjects)
            => ExcelAsyncUtil.Observe(
                    collerFunctionName,
                    paramObjects,
                    () => ExcelDnaUtility.FuncOrNAIfThrown(() =>
                    {
                        return JsonUtility.NewJsonObjectHandle(objectRepository, jsonObjectFunc());
                    })
                    );
    }
}
