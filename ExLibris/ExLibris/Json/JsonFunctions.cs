using ExcelDna.Integration;
using ExLibris.Core;
using ExLibris.Core.Json;
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
                return ExcelAsyncUtil.Observe(
                    nameof(CreateJsonObject),
                    new object[] { param, },
                    () => ExcelDnaUtility.FuncOrNAIfThrown(() =>
                    {
                        var o = CreateJsonObjectByMatrix((object[,])param, context);
                        return JsonUtility.NewJsonObjectHandle(context.ObjectRepository, o);
                    })
                    );
            }

            if (param is string)
            {
                return ExcelAsyncUtil.Observe(
                    nameof(CreateJsonObject),
                    new object[] { param, },
                    () => ExcelDnaUtility.FuncOrNAIfThrown(() =>
                    {
                        var o = CreateJsonObjectByJsonText((string)param, context);
                        return JsonUtility.NewJsonObjectHandle(context.ObjectRepository, o);
                    })
                    );
            }

            return ExcelAsyncUtil.Observe(
                nameof(CreateJsonObject),
                new object[] { param, },
                () => ExcelDnaUtility.FuncOrNAIfThrown(() =>
                {
                    var o = new JsonObjectBuilder(context.ObjectRepository)
                        .SetOnlyRootValue(ExcelDnaUtility.NullIfEmpty(param))
                        .BuildJsonObject();
                    return JsonUtility.NewJsonObjectHandle(context.ObjectRepository, o);
                })
                );
        }

        private static object CreateJsonObjectByMatrix(object[,] matrix, ExLibrisContext context)
        {
            var rsize = matrix.GetLength(0);

            var job = new JsonObjectBuilder(context.ObjectRepository);

            for(var r = 0; r < rsize; ++r)
            {
                ExcelDnaUtility.ThrowIfMissingOrErrorOrEmpty(matrix[r, 0], () => $"matrix[{r}][0]");
                ExcelDnaUtility.ThrowIfMissingOrError(matrix[r, 1], () => $"matrix[{r}][1]");

                var keypath = (string)matrix[r, 0];
                var value = ExcelDnaUtility.NullIfEmpty(matrix[r, 1]);

                ExcelDnaUtility.ThrowIfMissingOrError(value, () => $"matrix[{r}][1]");

                job.AddJsonValue(keypath, value);
            }

            return job.BuildJsonObject();
        }

        private static object CreateJsonObjectByJsonText(string jsonText, ExLibrisContext context)
        {
            return JsonObjectSerialiser.ToJsonObject(jsonText) ??
                    new JsonObjectBuilder(context.ObjectRepository)
                    .SetOnlyRootValue(jsonText)
                    .BuildJsonObject();
        }


        [ExcelFunction(
            Name = "ExLibris.Json.ShowJsonText",
            Category = "ExLibris.Json"
            )]
        public static object ShowJsonText(string objectHandle, bool pretty)
            => ExcelDnaUtility.FuncOrNAIfThrown(() => CreateJsonObject(objectHandle, pretty, ExLibrisContext.DefaultContext));

        private static object CreateJsonObject(string objectHandle, bool pretty, ExLibrisContext context)
        {
            return ExcelAsyncUtil.Observe(
                nameof(CreateJsonObject),
                new object[]{ objectHandle, pretty, },
                () => ExcelDnaUtility.FuncOrNAIfThrown(() =>
                {
                    var o = pretty ?
                        JsonObjectSerialiser.ToJsonPrettyText(context.ObjectRepository.GetObject(objectHandle)) :
                        JsonObjectSerialiser.ToJsonText(context.ObjectRepository.GetObject(objectHandle));

                    return ExcelDnaUtility.NewSimpleExcelObservable(o);
                })
                );
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

            return ExcelAsyncUtil.Observe(
                nameof(GetJsonKeyValues),
                new object[] { objectHandle, },
                () => ExcelDnaUtility.FuncOrNAIfThrown(() =>
                {
                    var jo = context.ObjectRepository.GetObject(objectHandle);
                    var values = new JsonObjectAccessor(jo).GetJsonValues().ToList();

                    var excelvalues = new object[values.Count, 2];

                    for(var i = 0; i < values.Count; ++i)
                    {
                        excelvalues[i, 0] = values[i].KeyPath;
                        excelvalues[i, 1] = ExcelDnaUtility.ToExcelValue(values[i].Value);
                    }

                    return ExcelDnaUtility.NewSimpleExcelObservable(excelvalues);
                })
                );
        }
    }
}
