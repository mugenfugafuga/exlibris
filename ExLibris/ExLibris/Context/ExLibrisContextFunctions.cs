using ExcelDna.Integration;
using ExLibris.Core;
using ExLibris.Core.Json;
using System.Linq;

namespace ExLibris.Context
{
    public static class ExLibrisContextFunctions
    {
        [ExcelFunction(
            Name = "ExLibris.Context.ShowDefaultConfiguration",
            Category = "ExLibris.Context")]
        public static object ShowDefaultConfiguration()
        {
            var context = ExLibrisContext.DefaultContext;

            return ExLibrisUtility.RunAsync(
                nameof(ShowDefaultConfiguration),
                () => ToJsonKeyValues(context.DefaultExLibrisConfiguration),
                null);
        }

        private static object[,] ToJsonKeyValues(ExLibrisConfiguration configuration)
        {
            var values = new JsonObjectAccessor(JsonObjectSerialiser.ToJsonObject(configuration)).GetJsonValues().ToList();

            var excelvalues = new object[values.Count, 2];

            for (var i = 0; i < values.Count; ++i)
            {
                excelvalues[i, 0] = values[i].KeyPath;
                excelvalues[i, 1] = ExLibrisUtility.ToExcelValue(values[i].Value);
            }

            return excelvalues;
        }

    }
}
