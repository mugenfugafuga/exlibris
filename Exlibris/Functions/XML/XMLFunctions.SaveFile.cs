using ExcelDna.Integration;
using System.Xml;

namespace Exlibris.Functions.XML;
partial class XMLFunctions
{
    [ExcelFunction(
        Category = Category,
        Name = $"{Category}.{nameof(SaveFile)}",
        Description = "saves the xml string to a file using the specified xml management object.")]
    public static object SaveFile(
        [ExcelArgument(AllowReference = true, Description = "xml management object or xml Schema management object.")] object xml,
        [ExcelArgument(AllowReference = true, Description = "the path of the file where the xml string will be saved.")] object filePath,
        [ExcelArgument(AllowReference = true, Description = "optional argument. determines whether to indent the generated JSON or not. The default value is True, which means that the xml will be indented by default.")] object pretty,
        [ExcelArgument(Description = "optional argument.if the argument is invalid, the method will not execute the process.")] object identifier)
        => ExlibrisAddin.GetFunctionSupport().NoError(identifier).ErrorValueIfThrown(support =>
        {
            var xmlVal = support.NotDateTime(xml, nameof(xml)).ShouldBeScalar().GetValueOrThrow<XmlDocument>();
            var file = support.NotDateTime(filePath, nameof(filePath)).ShouldBeScalar().GetValueOrThrow<string>();
            var prtty = support.NotDateTime(pretty, nameof(pretty)).ShouldBeScalar().GetValueOrDefault(true);

            var setting = prtty ? IndentSetting : NoIndentSetting;

            using var writer = XmlWriter.Create(file, setting);
            xmlVal.Save(writer);

            return file;

            throw new NotImplementedException($"unsupported type. type : {xmlVal.Value?.GetType()}");
        });
}
