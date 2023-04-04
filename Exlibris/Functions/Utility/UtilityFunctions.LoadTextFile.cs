using ExcelDna.Integration;

namespace Exlibris.Functions.Utility;
partial class UtilityFunctions
{
    [ExcelFunction(
        Category = Category,
        Name = $"{Category}.{nameof(LoadTextFile)}",
        Description = "load text file",
        IsHidden = true)]
    public static object LoadTextFile(
        [ExcelArgument(Description = "file")] string filePath,
        [ExcelArgument(Description = "optional argument.if the argument is invalid, the method will not execute the process.")] object identifier)
        => ExlibrisAddin.GetFunctionSupport().NoError(identifier).ErrorValueIfThrown<object>(support =>
        {
            return File.ReadAllText(filePath);
        });
}
