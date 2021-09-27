using ExcelDna.Integration;
using ExLibris.Core;
using ExLibris.Core.Json;
using ExLibris.Json;
using System.Linq;

namespace ExLibris
{
    public static class ExLibrisFunctions
    {
        private const string categoryName = "ExLibris";
        private const string prefixFunctionName = categoryName + ".";

        [ExcelFunction(
            Name = prefixFunctionName + nameof(LoadConfigurationAsync),
            Category = categoryName)]
        public static object LoadConfigurationAsync(object[,] matrix)
        {
            var context = ExLibrisContext.DefaultContext;
            var support = context.GetFunctionCallSupport();

            return ExLibrisUtility.ExcelObserveObjectRegistrationAsync(
                nameof(LoadConfigurationAsync),
                support.ObjectRepository,
                () =>
                {
                    var ema = support.GetExcelMatrixAccessor(matrix);
                    var jo = JsonFunctions.CreateJsonObjectByMatrix(ema, support);

                    return JsonObjectSerialiser.ToObject<ExLibrisConfiguration>(jo);
                },
                matrix
                );
       }

        [ExcelFunction(
            Name = prefixFunctionName + nameof(DumpObjectAsync),
            Category = categoryName)]
        public static object DumpObjectAsync(string objectHandle)
        {
            var context = ExLibrisContext.DefaultContext;
            var support = context.GetFunctionCallSupport();

            return ExLibrisUtility.RunAsync(
                nameof(DumpObjectAsync),
                () => JsonFunctions.CreateJsonKeyValueTable(
                    JsonObjectSerialiser.ToJsonObject(
                        support.ObjectRepository.GetObject(objectHandle)),
                        support),
                objectHandle);

        }

        [ExcelFunction(
            Name = prefixFunctionName + nameof(GetRegisteredObjects),
            Category = categoryName)]
        public static object GetRegisteredObjects(object identifier)
        {
            var context = ExLibrisContext.DefaultContext;
            var support = context.GetFunctionCallSupport();

            return ExLibrisUtility.ObserveObject(
                nameof(GetRegisteredObjects),
                () => GetRegisteredObjects(support),
                identifier);
        }

        private static object[,] GetRegisteredObjects(ExcelFunctionCallSupport support)
        {
            var keys = support.ObjectRepository.Keys.ToArray();

            var vs = new object[keys.Length, 1];

            for(var i =0; i < keys.Length; ++i)
            {
                vs[i, 0] = support.ToValue(keys[i]);
            }

            return vs;
        }
    }
}
