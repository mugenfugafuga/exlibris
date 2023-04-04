using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;

namespace Exlibris.Core.JSONs.Tests
{
    partial class JSONParserTests
    {
        private readonly IJSONSerializer<JObject, JArray, JValue, JToken, JSchema> parser = JSONParserManager.GetJSONParser();

        [TestMethod()]
        public void FromJsonString_JSONNet_JSONObject()
        {
            var jo = parser.Deserialize(jsonString1);

            Assert.IsTrue(jo is JObject);

            var values = parser.GetValues(jo).ToArray();

            Assert.AreEqual(9, values.Length);
            { var v = values[0]; Assert.AreEqual("$.menu.id", v.Path); Assert.AreEqual("file", v.Value.Value); }
            { var v = values[1]; Assert.AreEqual("$.menu.value", v.Path); Assert.AreEqual("File", v.Value.Value); }
            { var v = values[2]; Assert.AreEqual("$.menu.null_value", v.Path); Assert.IsNull(v.Value.Value); }
            { var v = values[3]; Assert.AreEqual("$.menu.popup.menuitem[0].value", v.Path); Assert.AreEqual("New", v.Value.Value); }
            { var v = values[4]; Assert.AreEqual("$.menu.popup.menuitem[0].onclick", v.Path); Assert.AreEqual("CreateNewDoc()", v.Value.Value); }
            { var v = values[5]; Assert.AreEqual("$.menu.popup.menuitem[1].value", v.Path); Assert.AreEqual("Open", v.Value.Value); }
            { var v = values[6]; Assert.AreEqual("$.menu.popup.menuitem[1].onclick", v.Path); Assert.AreEqual("OpenDoc()", v.Value.Value); }
            { var v = values[7]; Assert.AreEqual("$.menu.popup.menuitem[2].value", v.Path); Assert.AreEqual("Close", v.Value.Value); }
            { var v = values[8]; Assert.AreEqual("$.menu.popup.menuitem[2].onclick", v.Path); Assert.AreEqual("CloseDoc()", v.Value.Value); }
        }

        [TestMethod()]
        public void FromJsonString_JSONNet_JSONArray()
        {
            var jo = parser.Deserialize(jsonArray1);

            Assert.IsTrue(jo is JArray);

            var values = parser.GetValues(jo).ToArray();

            Assert.AreEqual(67, values.Length);
            { var v = values[0]; Assert.AreEqual("$[0].servlet-name", v.Path); Assert.AreEqual("cofaxCDS", v.Value.Value); }
            { var v = values[1]; Assert.AreEqual("$[0].servlet-class", v.Path); Assert.AreEqual("org.cofax.cds.CDSServlet", v.Value.Value); }
            { var v = values[2]; Assert.AreEqual("$[0].init-param.configGlossary:installationAt", v.Path); Assert.AreEqual("Philadelphia, PA", v.Value.Value); }
            { var v = values[3]; Assert.AreEqual("$[0].init-param.configGlossary:adminEmail", v.Path); Assert.AreEqual("ksm@pobox.com", v.Value.Value); }
            { var v = values[4]; Assert.AreEqual("$[0].init-param.configGlossary:poweredBy", v.Path); Assert.AreEqual("Cofax", v.Value.Value); }
            { var v = values[5]; Assert.AreEqual("$[0].init-param.configGlossary:poweredByIcon", v.Path); Assert.AreEqual("/images/cofax.gif", v.Value.Value); }
            { var v = values[6]; Assert.AreEqual("$[0].init-param.configGlossary:staticPath", v.Path); Assert.AreEqual("/content/static", v.Value.Value); }
            { var v = values[7]; Assert.AreEqual("$[0].init-param.templateProcessorClass", v.Path); Assert.AreEqual("org.cofax.WysiwygTemplate", v.Value.Value); }
            { var v = values[8]; Assert.AreEqual("$[0].init-param.templateLoaderClass", v.Path); Assert.AreEqual("org.cofax.FilesTemplateLoader", v.Value.Value); }
            { var v = values[9]; Assert.AreEqual("$[0].init-param.templatePath", v.Path); Assert.AreEqual("templates", v.Value.Value); }
            { var v = values[10]; Assert.AreEqual("$[0].init-param.templateOverridePath", v.Path); Assert.AreEqual("", v.Value.Value); }
            { var v = values[11]; Assert.AreEqual("$[0].init-param.defaultListTemplate", v.Path); Assert.AreEqual("listTemplate.htm", v.Value.Value); }
            { var v = values[12]; Assert.AreEqual("$[0].init-param.defaultFileTemplate", v.Path); Assert.AreEqual("articleTemplate.htm", v.Value.Value); }
            { var v = values[13]; Assert.AreEqual("$[0].init-param.useJSP", v.Path); Assert.IsFalse((bool?)v.Value.Value); }
            { var v = values[14]; Assert.AreEqual("$[0].init-param.jspListTemplate", v.Path); Assert.AreEqual("listTemplate.jsp", v.Value.Value); }
            { var v = values[15]; Assert.AreEqual("$[0].init-param.jspFileTemplate", v.Path); Assert.AreEqual("articleTemplate.jsp", v.Value.Value); }
            { var v = values[16]; Assert.AreEqual("$[0].init-param.cachePackageTagsTrack", v.Path); Assert.AreEqual((long)200, v.Value.Value); }
            { var v = values[17]; Assert.AreEqual("$[0].init-param.cachePackageTagsStore", v.Path); Assert.AreEqual((long)200, v.Value.Value); }
            { var v = values[18]; Assert.AreEqual("$[0].init-param.cachePackageTagsRefresh", v.Path); Assert.AreEqual((long)60, v.Value.Value); }
            { var v = values[19]; Assert.AreEqual("$[0].init-param.cacheTemplatesTrack", v.Path); Assert.AreEqual((long)100, v.Value.Value); }
            { var v = values[20]; Assert.AreEqual("$[0].init-param.cacheTemplatesStore", v.Path); Assert.AreEqual((long)50, v.Value.Value); }
            { var v = values[21]; Assert.AreEqual("$[0].init-param.cacheTemplatesRefresh", v.Path); Assert.AreEqual((long)15, v.Value.Value); }
            { var v = values[22]; Assert.AreEqual("$[0].init-param.cachePagesTrack", v.Path); Assert.AreEqual((long)200, v.Value.Value); }
            { var v = values[23]; Assert.AreEqual("$[0].init-param.cachePagesStore", v.Path); Assert.AreEqual((long)100, v.Value.Value); }
            { var v = values[24]; Assert.AreEqual("$[0].init-param.cachePagesRefresh", v.Path); Assert.AreEqual((long)10, v.Value.Value); }
            { var v = values[25]; Assert.AreEqual("$[0].init-param.cachePagesDirtyRead", v.Path); Assert.AreEqual((long)10, v.Value.Value); }
            { var v = values[26]; Assert.AreEqual("$[0].init-param.searchEngineListTemplate", v.Path); Assert.AreEqual("forSearchEnginesList.htm", v.Value.Value); }
            { var v = values[27]; Assert.AreEqual("$[0].init-param.searchEngineFileTemplate", v.Path); Assert.AreEqual("forSearchEngines.htm", v.Value.Value); }
            { var v = values[28]; Assert.AreEqual("$[0].init-param.searchEngineRobotsDb", v.Path); Assert.AreEqual("WEB-INF/robots.db", v.Value.Value); }
            { var v = values[29]; Assert.AreEqual("$[0].init-param.useDataStore", v.Path); Assert.IsTrue((bool?)v.Value.Value); }
            { var v = values[30]; Assert.AreEqual("$[0].init-param.dataStoreClass", v.Path); Assert.AreEqual("org.cofax.SqlDataStore", v.Value.Value); }
            { var v = values[31]; Assert.AreEqual("$[0].init-param.redirectionClass", v.Path); Assert.AreEqual("org.cofax.SqlRedirection", v.Value.Value); }
            { var v = values[32]; Assert.AreEqual("$[0].init-param.dataStoreName", v.Path); Assert.AreEqual("cofax", v.Value.Value); }
            { var v = values[33]; Assert.AreEqual("$[0].init-param.dataStoreDriver", v.Path); Assert.AreEqual("com.microsoft.jdbc.sqlserver.SQLServerDriver", v.Value.Value); }
            { var v = values[34]; Assert.AreEqual("$[0].init-param.dataStoreUrl", v.Path); Assert.AreEqual("jdbc:microsoft:sqlserver://LOCALHOST:1433;DatabaseName=goon", v.Value.Value); }
            { var v = values[35]; Assert.AreEqual("$[0].init-param.dataStoreUser", v.Path); Assert.AreEqual("sa", v.Value.Value); }
            { var v = values[36]; Assert.AreEqual("$[0].init-param.dataStorePassword", v.Path); Assert.AreEqual("dataStoreTestQuery", v.Value.Value); }
            { var v = values[37]; Assert.AreEqual("$[0].init-param.dataStoreTestQuery", v.Path); Assert.AreEqual("SET NOCOUNT ON;select test='test';", v.Value.Value); }
            { var v = values[38]; Assert.AreEqual("$[0].init-param.dataStoreLogFile", v.Path); Assert.AreEqual("/usr/local/tomcat/logs/datastore.log", v.Value.Value); }
            { var v = values[39]; Assert.AreEqual("$[0].init-param.dataStoreInitConns", v.Path); Assert.AreEqual((long)10, v.Value.Value); }
            { var v = values[40]; Assert.AreEqual("$[0].init-param.dataStoreMaxConns", v.Path); Assert.AreEqual((long)100, v.Value.Value); }
            { var v = values[41]; Assert.AreEqual("$[0].init-param.dataStoreConnUsageLimit", v.Path); Assert.AreEqual((long)100, v.Value.Value); }
            { var v = values[42]; Assert.AreEqual("$[0].init-param.dataStoreLogLevel", v.Path); Assert.AreEqual("debug", v.Value.Value); }
            { var v = values[43]; Assert.AreEqual("$[0].init-param.maxUrlLength", v.Path); Assert.AreEqual((long)500, v.Value.Value); }
            { var v = values[44]; Assert.AreEqual("$[1].servlet-name", v.Path); Assert.AreEqual("cofaxEmail", v.Value.Value); }
            { var v = values[45]; Assert.AreEqual("$[1].servlet-class", v.Path); Assert.AreEqual("org.cofax.cds.EmailServlet", v.Value.Value); }
            { var v = values[46]; Assert.AreEqual("$[1].init-param.mailHost", v.Path); Assert.AreEqual("mail1", v.Value.Value); }
            { var v = values[47]; Assert.AreEqual("$[1].init-param.mailHostOverride", v.Path); Assert.AreEqual("mail2", v.Value.Value); }
            { var v = values[48]; Assert.AreEqual("$[2].servlet-name", v.Path); Assert.AreEqual("cofaxAdmin", v.Value.Value); }
            { var v = values[49]; Assert.AreEqual("$[2].servlet-class", v.Path); Assert.AreEqual("org.cofax.cds.AdminServlet", v.Value.Value); }
            { var v = values[50]; Assert.AreEqual("$[3].servlet-name", v.Path); Assert.AreEqual("fileServlet", v.Value.Value); }
            { var v = values[51]; Assert.AreEqual("$[3].servlet-class", v.Path); Assert.AreEqual("org.cofax.cds.FileServlet", v.Value.Value); }
            { var v = values[52]; Assert.AreEqual("$[4].servlet-name", v.Path); Assert.AreEqual("cofaxTools", v.Value.Value); }
            { var v = values[53]; Assert.AreEqual("$[4].servlet-class", v.Path); Assert.AreEqual("org.cofax.cms.CofaxToolsServlet", v.Value.Value); }
            { var v = values[54]; Assert.AreEqual("$[4].init-param.templatePath", v.Path); Assert.AreEqual("toolstemplates/", v.Value.Value); }
            { var v = values[55]; Assert.AreEqual("$[4].init-param.log", v.Path); Assert.AreEqual((long)1, v.Value.Value); }
            { var v = values[56]; Assert.AreEqual("$[4].init-param.logLocation", v.Path); Assert.AreEqual("/usr/local/tomcat/logs/CofaxTools.log", v.Value.Value); }
            { var v = values[57]; Assert.AreEqual("$[4].init-param.logMaxSize", v.Path); Assert.AreEqual("", v.Value.Value); }
            { var v = values[58]; Assert.AreEqual("$[4].init-param.dataLog", v.Path); Assert.AreEqual((long)1, v.Value.Value); }
            { var v = values[59]; Assert.AreEqual("$[4].init-param.dataLogLocation", v.Path); Assert.AreEqual("/usr/local/tomcat/logs/dataLog.log", v.Value.Value); }
            { var v = values[60]; Assert.AreEqual("$[4].init-param.dataLogMaxSize", v.Path); Assert.AreEqual("", v.Value.Value); }
            { var v = values[61]; Assert.AreEqual("$[4].init-param.removePageCache", v.Path); Assert.AreEqual("/content/admin/remove?cache=pages&id=", v.Value.Value); }
            { var v = values[62]; Assert.AreEqual("$[4].init-param.removeTemplateCache", v.Path); Assert.AreEqual("/content/admin/remove?cache=templates&id=", v.Value.Value); }
            { var v = values[63]; Assert.AreEqual("$[4].init-param.fileTransferFolder", v.Path); Assert.AreEqual("/usr/local/tomcat/webapps/content/fileTransferFolder", v.Value.Value); }
            { var v = values[64]; Assert.AreEqual("$[4].init-param.lookInContext", v.Path); Assert.AreEqual((long)1, v.Value.Value); }
            { var v = values[65]; Assert.AreEqual("$[4].init-param.adminGroupID", v.Path); Assert.AreEqual((long)4, v.Value.Value); }
            { var v = values[66]; Assert.AreEqual("$[4].init-param.betaServer", v.Path); Assert.IsTrue((bool?)v.Value.Value); }
        }

        [TestMethod()]
        public void FromJsonString_JSONNet_JSONValue_String()
        {
            var jo = parser.Deserialize(jsonValueString);
            Assert.IsTrue(jo is JValue);

            var values = parser.GetValues(jo).ToArray();
            Assert.AreEqual(1, values.Length);
            { var v = values[0]; Assert.AreEqual("$", v.Path); Assert.AreEqual("json string", v.Value.Value); }
        }

        [TestMethod()]
        public void FromJsonString_JSONNet_JSONValue_Int()
        {
            var jo = parser.Deserialize(jsonValueInt);
            Assert.IsTrue(jo is JValue);

            var values = parser.GetValues(jo).ToArray();
            Assert.AreEqual(1, values.Length);
            { var v = values[0]; Assert.AreEqual("$", v.Path); Assert.AreEqual((long)-123, v.Value.Value); }
            { var v = values[0]; Assert.AreEqual("$", v.Path); Assert.AreEqual(-123.0, v.Value.Value<double>()); }
        }

        [TestMethod()]
        public void FromJsonString_JSONNet_JSONValue_Double()
        {
            var jo = parser.Deserialize(jsonValueDouble);
            Assert.IsTrue(jo is JValue);

            var values = parser.GetValues(jo).ToArray();
            Assert.AreEqual(1, values.Length);
            { var v = values[0]; Assert.AreEqual("$", v.Path); Assert.AreEqual(-123.456, v.Value.Value); }
        }

        [TestMethod()]
        public void FromJsonString_JSONNet_JSONValue_True()
        {
            var jo = parser.Deserialize(jsonValueTrue);
            Assert.IsTrue(jo is JValue);

            var values = parser.GetValues(jo).ToArray();
            Assert.AreEqual(1, values.Length);
            { var v = values[0]; Assert.AreEqual("$", v.Path); Assert.IsTrue((bool?)v.Value.Value); }
        }

        [TestMethod()]
        public void FromJsonString_JSONNet_JSONValue_False()
        {
            var jo = parser.Deserialize(jsonValueFalse);
            Assert.IsTrue(jo is JValue);

            var values = parser.GetValues(jo).ToArray();
            Assert.AreEqual(1, values.Length);
            { var v = values[0]; Assert.AreEqual("$", v.Path); Assert.IsFalse((bool?)v.Value.Value); }
        }
    }
}