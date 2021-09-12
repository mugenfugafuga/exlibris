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
                    var jo = JsonFunctions.CreateJsonObjectByMatrix(ema, context, configuration.jsonObjectConfiguration.GetJsonValueConverter());

                    return JsonObjectSerialiser.ToObject<ExLibrisConfiguration>(jo);
                },
                matrix
                );
       }

        public static object DumpObject(string objectHandle)
        {
            var context = ExLibrisContext.DefaultContext;
            var configuration = context.DefaultExLibrisConfiguration;

            return ExLibrisUtility.RunAsync(
                nameof(DumpObject),
                () => JsonFunctions.CreateJsonKeyValueTable(
                    JsonObjectSerialiser.ToJsonObject(context.ObjectRepository.GetObject(objectHandle)),
                    ExLibrisUtility.GetExcelValueConverter(configuration)),
                objectHandle);

        }
    }
}
