using ExcelDna.Integration;
using System;

namespace Exlibris.Functions.Utility
{
    partial class UtilityFunctions
    {
        [ExcelFunction(
            Category = Category,
            Name = Category + ".DisplayTime",
            Description = "display the current time at regular intervals",
            IsHidden = true)]
        public static object DisplayTime(
            [ExcelArgument(Description = "type of time. 0 : double (default), 1 : string")] object timeType,
            [ExcelArgument(Description = "optional argument.if the argument is invalid, the method will not execute the process.")] object identifier)
            => ExlibrisAddin.GetFunctionSupport().NoError(identifier).ErrorValueIfThrown<object>(support =>
            {
                var tmtype = support.NotDateTime(timeType, nameof(timeType)).GetValueOrDefault(DisplayTimeType.Double);

                switch (tmtype)
                {
                    case DisplayTimeType.String: return DateTime.Now.ToString();
                    default: return DateTime.Now;
                }
            });
    }
}