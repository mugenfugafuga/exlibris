using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

namespace Exlibris.Core.JSONs.Tests
{
    [TestClass()]
    public partial class JSONParserTests
    {
        private static void GenerateAssertion(string path, JValue value, int index)
        {
            switch (value.Type)
            {
                case JTokenType.Null: Console.WriteLine($"{{ var v = values[{index}]; Assert.AreEqual(\"{path}\", v.Path); Assert.IsTrue(v.Value.Value == null); }}"); break;
                case JTokenType.String: Console.WriteLine($"{{ var v = values[{index}]; Assert.AreEqual(\"{path}\", v.Path); Assert.AreEqual(\"{value}\", v.Value.Value); }}"); break;
                case JTokenType.Boolean:
                    {
                        Debug.Assert(value.Value != null);
                        if ((bool)value.Value)
                        {
                            Console.WriteLine($"{{ var v = values[{index}]; Assert.AreEqual(\"{path}\", v.Path); Assert.IsTrue((bool?)v.Value.Value); }}");
                        }
                        else
                        {
                            Console.WriteLine($"{{ var v = values[{index}]; Assert.AreEqual(\"{path}\", v.Path); Assert.IsFalse((bool?)v.Value.Value); }}");
                        }
                        break;
                    }
                case JTokenType.Integer: Console.WriteLine($"{{ var v = values[{index}]; Assert.AreEqual(\"{path}\", v.Path); Assert.AreEqual((long){value}, v.Value.Value); }}"); break;
                default: Console.WriteLine($"{{ var v = values[{index}]; Assert.AreEqual(\"{path}\", v.Path); Assert.AreEqual({value}, v.Value.Value); }}"); break;
            }
        }

        private static void GenerateAssertions(IEnumerable<(string Path, JValue Value)> values)
        {
            var vals = values.ToArray();

            Console.WriteLine($"Assert.AreEqual({vals.Length}, values.Length);");

            var i = 0;
            foreach(var (path, value) in  vals)
            {
                GenerateAssertion(path, value, i++);
            }
        }

        private const string jsonValueString = @"""json string""";
        private const string jsonValueInt = @"-123";
        private const string jsonValueDouble = @"-123.456";
        private const string jsonValueTrue = @"true";
        private const string jsonValueFalse = @"false";

        private const string jsonString1 =
@"{
  ""menu"": {
    ""id"": ""file"",
    ""value"": ""File"",
    ""null_value"" : null,
    ""popup"": {
      ""menuitem"": [
        {
          ""value"": ""New"",
          ""onclick"": ""CreateNewDoc()""
        },
        {
          ""value"": ""Open"",
          ""onclick"": ""OpenDoc()""
        },
        {
          ""value"": ""Close"",
          ""onclick"": ""CloseDoc()""
        }
      ]
    }
  }
}";

        private const string jsonArray1 =
@"[   
    {
      ""servlet-name"": ""cofaxCDS"",
      ""servlet-class"": ""org.cofax.cds.CDSServlet"",
      ""init-param"": {
        ""configGlossary:installationAt"": ""Philadelphia, PA"",
        ""configGlossary:adminEmail"": ""ksm@pobox.com"",
        ""configGlossary:poweredBy"": ""Cofax"",
        ""configGlossary:poweredByIcon"": ""/images/cofax.gif"",
        ""configGlossary:staticPath"": ""/content/static"",
        ""templateProcessorClass"": ""org.cofax.WysiwygTemplate"",
        ""templateLoaderClass"": ""org.cofax.FilesTemplateLoader"",
        ""templatePath"": ""templates"",
        ""templateOverridePath"": """",
        ""defaultListTemplate"": ""listTemplate.htm"",
        ""defaultFileTemplate"": ""articleTemplate.htm"",
        ""useJSP"": false,
        ""jspListTemplate"": ""listTemplate.jsp"",
        ""jspFileTemplate"": ""articleTemplate.jsp"",
        ""cachePackageTagsTrack"": 200,
        ""cachePackageTagsStore"": 200,
        ""cachePackageTagsRefresh"": 60,
        ""cacheTemplatesTrack"": 100,
        ""cacheTemplatesStore"": 50,
        ""cacheTemplatesRefresh"": 15,
        ""cachePagesTrack"": 200,
        ""cachePagesStore"": 100,
        ""cachePagesRefresh"": 10,
        ""cachePagesDirtyRead"": 10,
        ""searchEngineListTemplate"": ""forSearchEnginesList.htm"",
        ""searchEngineFileTemplate"": ""forSearchEngines.htm"",
        ""searchEngineRobotsDb"": ""WEB-INF/robots.db"",
        ""useDataStore"": true,
        ""dataStoreClass"": ""org.cofax.SqlDataStore"",
        ""redirectionClass"": ""org.cofax.SqlRedirection"",
        ""dataStoreName"": ""cofax"",
        ""dataStoreDriver"": ""com.microsoft.jdbc.sqlserver.SQLServerDriver"",
        ""dataStoreUrl"": ""jdbc:microsoft:sqlserver://LOCALHOST:1433;DatabaseName=goon"",
        ""dataStoreUser"": ""sa"",
        ""dataStorePassword"": ""dataStoreTestQuery"",
        ""dataStoreTestQuery"": ""SET NOCOUNT ON;select test='test';"",
        ""dataStoreLogFile"": ""/usr/local/tomcat/logs/datastore.log"",
        ""dataStoreInitConns"": 10,
        ""dataStoreMaxConns"": 100,
        ""dataStoreConnUsageLimit"": 100,
        ""dataStoreLogLevel"": ""debug"",
        ""maxUrlLength"": 500}},
    {
      ""servlet-name"": ""cofaxEmail"",
      ""servlet-class"": ""org.cofax.cds.EmailServlet"",
      ""init-param"": {
      ""mailHost"": ""mail1"",
      ""mailHostOverride"": ""mail2""}},
    {
      ""servlet-name"": ""cofaxAdmin"",
      ""servlet-class"": ""org.cofax.cds.AdminServlet""},
 
    {
      ""servlet-name"": ""fileServlet"",
      ""servlet-class"": ""org.cofax.cds.FileServlet""},
    {
      ""servlet-name"": ""cofaxTools"",
      ""servlet-class"": ""org.cofax.cms.CofaxToolsServlet"",
      ""init-param"": {
        ""templatePath"": ""toolstemplates/"",
        ""log"": 1,
        ""logLocation"": ""/usr/local/tomcat/logs/CofaxTools.log"",
        ""logMaxSize"": """",
        ""dataLog"": 1,
        ""dataLogLocation"": ""/usr/local/tomcat/logs/dataLog.log"",
        ""dataLogMaxSize"": """",
        ""removePageCache"": ""/content/admin/remove?cache=pages&id="",
        ""removeTemplateCache"": ""/content/admin/remove?cache=templates&id="",
        ""fileTransferFolder"": ""/usr/local/tomcat/webapps/content/fileTransferFolder"",
        ""lookInContext"": 1,
        ""adminGroupID"": 4,
        ""betaServer"": true}}]";
    }
}