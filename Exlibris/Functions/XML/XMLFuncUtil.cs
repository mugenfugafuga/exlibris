using Exlibris.Excel;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Xml;

namespace Exlibris.Functions.XML
{
    static class XMLFuncUtil
    {
        public static XmlDocument CreateXMLObject(ExlibrisExcelFunctionSupport support, IExcelValue value)
        {
            if (value.TryGetValue<JToken>(out var json))
            {
                return support.JSONSerializer.ToXml(json);
            }

            return CreateXMLObjectFromString(value.ShouldBeScalar().GetValueOrThrow<string>());
        }


        public static XmlDocument CreateXMLObjectFromString(string value)
        {
            if (File.Exists(value))
            {
                var doc = new XmlDocument();
                doc.Load(value);
                return doc;
            }

            {
                var doc = new XmlDocument();
                doc.LoadXml(value);
                return doc;
            }
        }
    }
}