using System;
using System.Xml;

namespace Exlibris.Functions.XML
{
    public static partial class XMLFunctions
    {
        private const string Category = "Exlibris.XML";

        private static readonly XmlWriterSettings IndentSetting = new XmlWriterSettings()
        {
            Indent = true,
            NewLineChars = Environment.NewLine,
        };

        private static readonly XmlWriterSettings NoIndentSetting = new XmlWriterSettings()
        {
            Indent = false,
        };
    }
}