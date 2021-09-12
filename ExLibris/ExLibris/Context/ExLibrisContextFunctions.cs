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
                () => ToJsonKeyValues(
                    context.DefaultExLibrisConfiguration,
                    ExLibrisUtility.GetExcelValueConverter(context.DefaultExLibrisConfiguration)),
                null);
        }

        private static object[,] ToJsonKeyValues(ExLibrisConfiguration configuration, ExcelValueConverter valueConverter)
        {
            var values = new JsonObjectAccessor(JsonObjectSerialiser.ToJsonObject(configuration)).GetJsonValues().ToList();

            var mb = valueConverter.GetExcelMatrixBuilder(values.Count, 2);

            for (var i = 0; i < values.Count; ++i)
            {
                mb[i, 0] = values[i].KeyPath;
                mb[i, 1] = values[i].Value;
            }

            return mb.BuildExcelMatrix();
        }

    }
}
