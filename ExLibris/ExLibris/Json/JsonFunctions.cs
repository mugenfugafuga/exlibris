using ExcelDna.Integration;
using ExLibris.Core;
using ExLibris.Core.Json;

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

            return ExcelError.ExcelErrorNA;
        }

        private static object CreateJsonObjectByMatrix(object[,] matrix, ExLibrisContext context)
        {
            var rsize = matrix.GetLength(0);

            var job = new JsonObjectBuilder(context.ObjectRepository);

            for(var r = 0; r < rsize; ++r)
            {
                var keypath = matrix[r, 0].ToString();
                var value = matrix[r, 1];

                ExcelDnaUtility.ThrowIfMissingOrError(value, () => $"matrix[{r}][1]");

                job.AddJsonValue(keypath, value);
            }

            return job.BuildJsonObject();
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
                        JsonObjectSerialiser.Serialize(context.ObjectRepository.GetObject(objectHandle));

                    return ExcelDnaUtility.NewSimpleExcelObservable(o);
                })
                );
        }

    }
}
