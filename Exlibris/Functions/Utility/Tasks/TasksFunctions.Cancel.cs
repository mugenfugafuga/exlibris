using ExcelDna.Integration;
using System.Threading;
using System;

namespace Exlibris.Functions.Utility.Tasks
{
    partial class TasksFunctions
    {
        [ExcelFunction(
        Category = Category,
            Name = Category + ".Cancel",
        Description = "show the task status")]
        public static object Cancel(
            [ExcelArgument(AllowReference = true, Description = "target cell")] object targetCell,
            [ExcelArgument(Description = "optional argument.if the argument is invalid, the method will not execute the process.")] object identifier)
            => ExlibrisAddin.GetFunctionSupport().NoError(identifier).ErrorValueIfThrown(support =>
            {
                var address = ExlibrisUtil.GetRawExcel(targetCell, nameof(targetCell)).Address ?? throw new ArgumentException($"{nameof(targetCell)} is not cell.");

                support.GetCachedObject<CancellationTokenSource>(address).Cancel();

                return true;
            });
    }
}