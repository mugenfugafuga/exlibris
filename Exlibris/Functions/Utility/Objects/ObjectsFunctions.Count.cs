using ExcelDna.Integration;

namespace Exlibris.Functions.Utility.Objects;

partial class ObjectsFunctions
{
    [ExcelFunction(
        Category = Category,
        Name = $"{Category}.{nameof(Count)}",
        Description = "count registered objects",
        IsHidden = true)]
    public static object Count(
        [ExcelArgument(Description = "optional argument.if the argument is invalid, the method will not execute the process.")] object identifier)
        => ExlibrisAddin.GetFunctionSupport().NoError(identifier).ErrorValueIfThrown(support => {
            return support.ObjectRegistry.Count;
        });
}
