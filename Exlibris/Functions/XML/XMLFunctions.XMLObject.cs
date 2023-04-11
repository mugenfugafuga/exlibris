using ExcelDna.Integration;

namespace Exlibris.Functions.XML;
partial class XMLFunctions
{
    [ExcelFunction(
        Category = Category,
        Name = $"{Category}.{nameof(XMLObject)}",
        Description = "generates a XML management object in memory.")]
    public static object XMLObject(
        [ExcelArgument(AllowReference = true, Description = "XML string or file path.")] object source,
        [ExcelArgument(Description = "optional argument.if the argument is invalid, the method will not execute the process.")] object identifier
        )
        => ExlibrisAddin.GetFunctionSupport().NoError(identifier).ErrorValueIfThrown(support =>
        {
            var src = support.NotDateTime(source, nameof(source));

            return support.ObserveObjectRegistration(
                () =>
                {
                    return XMLFuncUtil.CreateXMLObject(support, src);
                }, source, identifier);
        });
}
