using System.Xml;

namespace Exlibris.Functions.XML;
public static partial class XMLFunctions
{
    private const string Category = $"{nameof(Exlibris)}.{nameof(XML)}";

    private static readonly XmlWriterSettings IndentSetting = new()
    {
        Indent = true,
        NewLineChars = Environment.NewLine,
    };

    private static readonly XmlWriterSettings NoIndentSetting = new()
    {
        Indent = false,
    };
}
