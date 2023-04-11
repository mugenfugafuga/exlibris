using ExcelDna.Integration;

namespace Exlibris.Functions.Utility;
partial class UtilityFunctions
{
    [ExcelFunction(
        Category = Category,
        Name = $"{Category}.{nameof(LoadText)}",
        Description = "load text file")]
    public static object LoadText(
        [ExcelArgument(AllowReference = true, Description = "the path of text file")] object filePath,
        [ExcelArgument(AllowReference = true, Description = "setting the argument to ‘true’ loads the text file as an object in the memory, while setting it to ‘false’ displays the text on Excel.")] object asObject,
        [ExcelArgument(Description = "optional argument.if the argument is invalid, the method will not execute the process.")] object identifier)
        => ExlibrisAddin.GetFunctionSupport().NoError(identifier).ErrorValueIfThrown<object>(support =>
        {
            var file = support.NotDateTime(filePath, nameof(filePath)).ShouldBeScalar().GetValueOrThrow<string>();

            if (support.NotDateTime(asObject, nameof(asObject)).ShouldBeScalar().GetValueOrDefault(true))
            {
                return support.ObserveObjectRegistration(() =>
                {
                    return File.ReadAllText(file);
                }, filePath, asObject, identifier);
            }
            else
            {
                return File.ReadAllText(file);
            }
        });
}
