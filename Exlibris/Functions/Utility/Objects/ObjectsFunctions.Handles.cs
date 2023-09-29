using ExcelDna.Integration;
using Exlibris.Excel;

namespace Exlibris.Functions.Utility.Objects
{
    partial class ObjectsFunctions
    {
        [ExcelFunction(
            Category = Category,
            Name = Category + ".Handles",
            Description = "show registered object handles",
            IsHidden = true)]
        public static object Handles(
            [ExcelArgument(Description = "optional argument.if the argument is invalid, the method will not execute the process.")] object identifier)
            => ExlibrisAddin.GetFunctionSupport().NoError(identifier).ErrorValueIfThrown(support =>
            {
                var builder = support.NewMatrixBuilder();

                foreach (var (_, handle) in support.ObjectRegistry.Handles)
                {
                    builder.NewRow().Add(handle.Key).Add((handle.Misc as ExcelAddress?)?.Address).Close();
                }

                return builder.Build();
            });
    }
}