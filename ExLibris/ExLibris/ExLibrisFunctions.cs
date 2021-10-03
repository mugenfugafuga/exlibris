using ExcelDna.Integration;
using ExLibris.Core;
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
                () => support.CreateObject<ExLibrisConfiguration>(matrix),
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
                () => support.CreateJsonKeyValueMatrix(support.ObjectRepository.GetObject(objectHandle)),
                objectHandle);

        }

        [ExcelFunction(
            Name = prefixFunctionName + nameof(GetRegisteredObjects),
            Category = categoryName)]
        public static object GetRegisteredObjectsAsync(object identifier)
        {
            var context = ExLibrisContext.DefaultContext;
            var support = context.GetFunctionCallSupport();

            return ExLibrisUtility.ExcelObserveObjectAsync(
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
