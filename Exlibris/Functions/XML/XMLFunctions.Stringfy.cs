using ExcelDna.Integration;
using System.Xml;

namespace Exlibris.Functions.XML;
partial class XMLFunctions
{
    [ExcelFunction(
        Category = Category,
        Name = $"{Category}.{nameof(Stringfy)}",
        Description = "generates a xml string from the specified xml management object.")]
    public static object Stringfy(
        [ExcelArgument(AllowReference = true, Description = "JSON management object or xml Schema management object.")] object xml,
        [ExcelArgument(Description = "optional argument.if the argument is invalid, the method will not execute the process.")] object identifier)
        => ExlibrisAddin.GetFunctionSupport().NoError(identifier).ErrorValueIfThrown(support =>
        {
            return support.NotDateTime(xml, nameof(xml)).ShouldBeScalar().GetValueOrThrow<XmlDocument>().OuterXml;
        });
}
