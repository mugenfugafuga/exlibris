using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;

namespace Exlibris.Core.JSONs.Tests;

[TestClass()]
public class JSONObjectBuilderTests
{
    private static readonly IJSONSerializer<JObject, JArray, JValue, JToken, JSchema> parser = JSONParserManager.GetJSONParser();

    [TestMethod()]
    public void BuildTest_Only_Root()
    {
        var jt = parser.NewJSONBuilder()
            .Append("$", "root")
            .Build();

        Assert.AreEqual(@"root", jt.ToString());
    }

    [TestMethod()]
    public void BuildTest_Root_Array()
    {
        var jt = parser.NewJSONBuilder()
            .Append("$[1]", 1.0)
            .Append("$[3]", "test")
            .Append("$[2]", true)
            .Build();

        Assert.AreEqual(@"[null,1.0,true,""test""]", jt.ToString(Formatting.None));
    }

    [TestMethod()]
    public void BuildTest_Object()
    {
        var expected = parser.Deserialize(jsonString1);

        var actual = parser.GetValues(expected)
            .Aggregate(parser.NewJSONBuilder(), (bulder, val) => bulder.Append(val.Path, val.Value.Value))
            .Build();

        Assert.AreEqual(expected.ToString(), actual.ToString());
    }

    [TestMethod()]
    public void BuildTest_Array()
    {
        var expected = parser.Deserialize(jsonArray1);

        var actual = parser.GetValues(expected)
            .Aggregate(parser.NewJSONBuilder(), (bulder, val) => bulder.Append(val.Path, val.Value.Value))
            .Build();

        Assert.AreEqual(expected.ToString(), actual.ToString());
    }

    [TestMethod()]
    [ExpectedException(typeof(ArgumentException))]
    public void BuildTest_Root_NG()
    {
        parser.NewJSONBuilder()
            .Append("$", "root")
            .Append("$", 123.4);
    }

    [TestMethod()]
    [ExpectedException(typeof(ArgumentException))]
    public void BuildTest_Root_Array_NG()
    {
        parser.NewJSONBuilder()
            .Append("$", "root")
            .Append("$[1]", 123.4);
    }

    [TestMethod()]
    [ExpectedException(typeof(ArgumentException))]
    public void BuildTest_Root_Same_Array_Node_NG()
    {
        parser.NewJSONBuilder()
            .Append("$[1]", "root")
            .Append("$[1]", 123.4);
    }

    [TestMethod()]
    [ExpectedException(typeof(ArgumentException))]
    public void BuildTest_Root_Same_Object_Properties_NG()
    {
        parser.NewJSONBuilder()
            .Append("$.object.value", "root")
            .Append("$.object.value", false);
    }

    [TestMethod()]
    [ExpectedException(typeof(ArgumentException))]
    public void BuildTest_Object_Array_NG()
    {
        parser.NewJSONBuilder()
            .Append("$.object.value", "root")
            .Append("$.object[1].value", 123.4);
    }

    [TestMethod()]
    [ExpectedException(typeof(ArgumentException))]
    public void BuildTest_Array_Object_NG()
    {
        parser.NewJSONBuilder()
            .Append("$.object[1].value", 123.4)
            .Append("$.object.value", "root");
    }

    [TestMethod()]
    [ExpectedException(typeof(ArgumentException))]
    public void BuildTest_Leaf_Object_Array_NG()
    {
        parser.NewJSONBuilder()
            .Append("$.object.value", "root")
            .Append("$.object.value[2]", 123.4);
    }

    [TestMethod()]
    [ExpectedException(typeof(ArgumentException))]
    public void BuildTest_Leaf_Array_Object_NG()
    {
        parser.NewJSONBuilder()
            .Append("$.object.value[2]", 123.4)
            .Append("$.object.value", "root");
    }

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