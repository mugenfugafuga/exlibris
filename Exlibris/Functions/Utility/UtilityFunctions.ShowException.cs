using ExcelDna.Integration;

namespace Exlibris.Functions.Utility;

partial class UtilityFunctions
{
    [ExcelFunction(
        Category = Category,
        Name = $"{Category}.{nameof(ShowException)}",
        Description = "show the last exception thrown in the calculation of the target cell",
        IsHidden = true)]
    public static object ShowException(
        [ExcelArgument(AllowReference = true, Description = "target cell")] object errorCell,
        [ExcelArgument(Description = "optional argument.if the argument is invalid, the method will not execute the process.")] object identifier)
        => ExlibrisAddin.GetFunctionSupport().NoError(identifier).ErrorValueIfThrown<object>(support =>
        {
            var address = ExlibrisUtil.GetAddress(errorCell as ExcelReference) ?? throw new ArgumentNullException(nameof(errorCell));
            return support.GetThrownException(address)?.ToString() ?? throw new Exception($"no exception has occured at {address.Address}");
        });
}
