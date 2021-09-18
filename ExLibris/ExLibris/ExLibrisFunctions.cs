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
            var support = context.GetFunctionCallSupport();

            return ExLibrisUtility.ObserveObjectHandle(
                nameof(LoadConfiguration),
                support.ObjectRepository,
                () =>
                {
                    var ema = support.GetExcelValueConverter().GetExcelMatrixAccessor(matrix);
                    var jo = JsonFunctions.CreateJsonObjectByMatrix(ema, support, support.GetJsonValueConverter());

                    return JsonObjectSerialiser.ToObject<ExLibrisConfiguration>(jo);
                },
                matrix
                );
       }

        public static object DumpObject(string objectHandle)
        {
            var context = ExLibrisContext.DefaultContext;
            var support = context.GetFunctionCallSupport();

            return ExLibrisUtility.RunAsync(
                nameof(DumpObject),
                () => JsonFunctions.CreateJsonKeyValueTable(
                    JsonObjectSerialiser.ToJsonObject(support.ObjectRepository.GetObject(objectHandle)),
                    support.GetExcelValueConverter()),
                objectHandle);

        }
    }
}
