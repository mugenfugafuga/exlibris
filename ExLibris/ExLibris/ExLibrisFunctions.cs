using ExcelDna.Integration;
using ExLibris.Core;
using ExLibris.Core.Json;
using ExLibris.Json;
using System.Linq;

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

            return ExLibrisUtility.ExcelObserveObjectRegistrationAsync(
                nameof(LoadConfiguration),
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
            Name = "ExLibris.DumpObject",
            Category = "ExLibris")]
        public static object DumpObject(string objectHandle)
        {
            var context = ExLibrisContext.DefaultContext;
            var support = context.GetFunctionCallSupport();

            return ExLibrisUtility.RunAsync(
                nameof(DumpObject),
                () => JsonFunctions.CreateJsonKeyValueTable(
                    JsonObjectSerialiser.ToJsonObject(
                        support.ObjectRepository.GetObject(objectHandle)),
                        support),
                objectHandle);

        }

        [ExcelFunction(
            Name = "ExLibris.GetRegisteredObjects",
            Category = "ExLibris")]
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
