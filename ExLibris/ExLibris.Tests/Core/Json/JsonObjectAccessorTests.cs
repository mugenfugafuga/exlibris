using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ExLibris.Core.Json.Tests
{
    [TestClass()]
    public class JsonObjectAccessorTests
    {
        [TestMethod()]
        public void JsonObjectAccessor_GetJsonValues_OnlyTopValue_Test()
        {
            var jo = new JsonObjectBuilder()
                .SetOnlyRootValue(false)
                .BuildJsonObject();

            var values = new JsonObjectAccessor(jo)
                .GetJsonValues()
                .OrderBy(kv => kv.KeyPath)
                .ToList();

            Assert.IsTrue(string.IsNullOrEmpty(values[0].KeyPath)); Assert.AreEqual(false, (bool)values[0].Value);
        }

        [TestMethod()]
        public void JsonObjectAccessor_GetJsonValuesTest()
        {
            var or = new ObjectRepository();

            {
                var orjo = new JsonObjectBuilder()
                    .AddJsonValue("vstring", "hoge")
                    .AddJsonValue("vint", 42)
                    .AddJsonValue("vdouble", -123.45)
                    .AddJsonValue("vtrue", true)
                    .AddJsonValue("vfalse", false)
                    .BuildJsonObject();

                or.RegisterObject("__jo", orjo);
            }

            var jo = new JsonObjectBuilder(or)
                .AddJsonValue("elem0", "__jo")
                .AddJsonValue("elem1.[0]", "__jo")
                .AddJsonValue("elem2", 42)
                .BuildJsonObject();

            var values = new JsonObjectAccessor(jo)
                .GetJsonValues()
                .OrderBy(kv => kv.KeyPath)
                .ToList();

            Assert.AreEqual("elem0.vdouble", values[0].KeyPath); Assert.AreEqual(-123.45d, (double)values[0].Value);
            Assert.AreEqual("elem0.vfalse", values[1].KeyPath); Assert.AreEqual(false, (bool)values[1].Value);
            Assert.AreEqual("elem0.vint", values[2].KeyPath); Assert.AreEqual(42, (int)values[2].Value);
            Assert.AreEqual("elem0.vstring", values[3].KeyPath); Assert.AreEqual("hoge", (string)values[3].Value);
            Assert.AreEqual("elem0.vtrue", values[4].KeyPath); Assert.AreEqual(true, (bool)values[4].Value);
            Assert.AreEqual("elem1.[0].vdouble", values[5].KeyPath); Assert.AreEqual(-123.45d, (double)values[5].Value);
            Assert.AreEqual("elem1.[0].vfalse", values[6].KeyPath); Assert.AreEqual(false, (bool)values[6].Value);
            Assert.AreEqual("elem1.[0].vint", values[7].KeyPath); Assert.AreEqual(42, (int)values[7].Value);
            Assert.AreEqual("elem1.[0].vstring", values[8].KeyPath); Assert.AreEqual("hoge", (string)values[8].Value);
            Assert.AreEqual("elem1.[0].vtrue", values[9].KeyPath); Assert.AreEqual(true, (bool)values[9].Value);
            Assert.AreEqual("elem2", values[10].KeyPath); Assert.AreEqual(42, (int)values[10].Value);
        }

        [TestMethod()]
        public void JsonObjectAccessor_GetJsonValueTest()
        {
            var or = new ObjectRepository();

            {
                var orjo = new JsonObjectBuilder()
                    .AddJsonValue("vstring", "hoge")
                    .AddJsonValue("vint", 42)
                    .AddJsonValue("vdouble", -123.45)
                    .AddJsonValue("vtrue", true)
                    .AddJsonValue("vfalse", false)
                    .BuildJsonObject();

                or.RegisterObject("__jo", orjo);
            }

            var jo = new JsonObjectBuilder(or)
                .AddJsonValue("elem0", "__jo")
                .AddJsonValue("elem1.[0]", "__jo")
                .AddJsonValue("elem2", 42)
                .BuildJsonObject();

            var accessor = new JsonObjectAccessor(jo);

            Assert.AreEqual(false, (bool)accessor.GetJsonValue("elem0.vfalse"));
            Assert.AreEqual("hoge", (string)accessor.GetJsonValue("elem1.[0].vstring"));

            Assert.IsTrue(accessor.GetJsonValue("elem0") is Dictionary<string, object>);
            Assert.IsTrue(accessor.GetJsonValue("elem1") is List<object>);
            Assert.IsTrue(accessor.GetJsonValue("null_value.hoge") == null);
        }

        [TestMethod()]
        public void JsonObjectAccessor_GetJsonValue_OnlyTopValue_Test()
        {
            var jo = new JsonObjectBuilder()
                .SetOnlyRootValue(false)
                .BuildJsonObject();

            Assert.IsFalse((bool)new JsonObjectAccessor(jo).GetJsonValue(null));
            Assert.IsFalse((bool)new JsonObjectAccessor(jo).GetJsonValue(string.Empty));
        }


        private static void CreateAssert(List<(string KeyPath, object Value)> values)
        {
            for(var i = 0; i < values.Count; ++i)
            {
                if (string .IsNullOrEmpty(values[i].KeyPath))
                {
                    Console.Write($"Assert.IsTrue(string.IsNullOrEmpty(values[{i}].KeyPath));");
                }
                else
                {
                    Console.Write($"Assert.AreEqual(\"{values[i].KeyPath}\", values[{i}].KeyPath);");
                }

                if (values[i].Value is string)
                {
                    Console.WriteLine($"Assert.AreEqual(\"{(string)values[i].Value}\", (string)values[{i}].Value);");
                }
                if (values[i].Value is int)
                {
                    Console.WriteLine($"Assert.AreEqual({(int)values[i].Value}, (int)values[{i}].Value);");
                }
                if (values[i].Value is double)
                {
                    Console.WriteLine($"Assert.AreEqual({(double)values[i].Value}d, (double)values[{i}].Value);");
                }
                if (values[i].Value is bool)
                {
                    var tf = (bool)values[i].Value ? "true" : "false";
                    Console.WriteLine($"Assert.AreEqual({tf}, (bool)values[{i}].Value);");
                }
            }
        }
    }
}