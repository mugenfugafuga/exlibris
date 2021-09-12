using ExcelDna.Integration;
using ExLibris.Core;
using ExLibris.Core.Json;
using ExLibris.Json;

namespace ExLibris
{
    public static class ExLibrisFunctions
    {
        [ExcelFunction(
            Name = "ExLibris.LoadConfiguration",
            Category = "ExLibris")]
        public static object LoadConfiguration(object[,] matrix)
        {
            var context = ExLibrisContext.DefaultContext;
            var configuration = context.DefaultExLibrisConfiguration;

            return ExLibrisUtility.ObserveObjectHandle(
                nameof(LoadConfiguration),
                context.ObjectRepository,
                () =>
                {
                    var ema = ExLibrisUtility.GetExcelValueConverter(configuration).GetExcelMatrixAccessor(matrix);
                    var jo = JsonFunctions.CreateJsonObjectByMatrix(ema, context);

                    return JsonObjectSerialiser.ToObject<ExLibrisConfiguration>(jo);
                },
                matrix
                );

        }
    }
}
