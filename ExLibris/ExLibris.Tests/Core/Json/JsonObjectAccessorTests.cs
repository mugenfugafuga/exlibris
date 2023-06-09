﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ExLibris.Core.Json.Tests
{
    [TestClass()]
    public class JsonObjectAccessorTests
    {
        private static JsonObjectBuilder NewJsonObjectBuilder()
        {
            var context = ExLibrisContext.DefaultContext;
            var support = context.GetFunctionCallSupport();

            return new JsonObjectBuilder(support.ObjectRepository, support.GetJsonValueConverter());
        }

        [TestMethod()]
        public void JsonObjectAccessor_GetJsonValues_OnlyTopValue_Test()
        {
            var jo = NewJsonObjectBuilder()
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
                var orjo = NewJsonObjectBuilder()
                    .AddJsonValue("vstring", "hoge")
                    .AddJsonValue("vint", 42)
                    .AddJsonValue("vdouble", -123.45)
                    .AddJsonValue("vtrue", true)
                    .AddJsonValue("vfalse", false)
                    .BuildJsonObject();

                or.RegisterObject("__jo", orjo);
            }

            var jo = new JsonObjectBuilder(or, new JsonObjectConfiguration().GetJsonValueConverter())
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
        public void JsonObjectAccessor_GetJsonValuesTest2()
        {
            var or = new ObjectRepository();

            {
                var orjo = NewJsonObjectBuilder()
                    .AddJsonValue("vstring", "hoge")
                    .AddJsonValue("vint", 42)
                    .AddJsonValue("vdouble", -123.45)
                    .AddJsonValue("vtrue", true)
                    .AddJsonValue("vfalse", false)
                    .BuildJsonObject();

                or.RegisterObject("__jo", orjo);
            }

            var jo = new JsonObjectBuilder(or, new JsonObjectConfiguration().GetJsonValueConverter())
                .AddJsonValue("elem0", "__jo")
                .AddJsonValue("elem1.[0]", "__jo")
                .AddJsonValue("elem2", 42)
                .BuildJsonObject();

            var joa = new JsonObjectAccessor(jo);

            var onlytop = joa
                .GetJsonValues("")
                .OrderBy(kv => kv.KeyPath)
                .ToList();

            Assert.AreEqual(JsonUtility.JsonRootKey, onlytop[0].KeyPath);
            Assert.IsTrue(JsonUtility.IsJsonDictionary(onlytop[0].Value));

            var twogen = joa
                .GetJsonValues("*.*")
                .OrderBy(kv => kv.KeyPath)
                .ToList();

            Assert.AreEqual(6, twogen.Count);
            Assert.AreEqual("elem0.vdouble", twogen[0].KeyPath); Assert.AreEqual(-123.45d, (double)twogen[0].Value);
            Assert.AreEqual("elem0.vfalse", twogen[1].KeyPath); Assert.AreEqual(false, (bool)twogen[1].Value);
            Assert.AreEqual("elem0.vint", twogen[2].KeyPath); Assert.AreEqual(42, (int)twogen[2].Value);
            Assert.AreEqual("elem0.vstring", twogen[3].KeyPath); Assert.AreEqual("hoge", (string)twogen[3].Value);
            Assert.AreEqual("elem0.vtrue", twogen[4].KeyPath); Assert.AreEqual(true, (bool)twogen[4].Value);
            Assert.AreEqual("elem1.[0]", twogen[5].KeyPath); Assert.IsTrue(JsonUtility.IsJsonDictionaryOrArray(twogen[5].Value));

            var thirdgen = joa
                .GetJsonValues("*.*.*")
                .OrderBy(kv => kv.KeyPath)
                .ToList();

            Assert.AreEqual(5, thirdgen.Count);
            Assert.AreEqual("elem1.[0].vdouble", thirdgen[0].KeyPath); Assert.AreEqual(-123.45d, (double)thirdgen[0].Value);
            Assert.AreEqual("elem1.[0].vfalse", thirdgen[1].KeyPath); Assert.AreEqual(false, (bool)thirdgen[1].Value);
            Assert.AreEqual("elem1.[0].vint", thirdgen[2].KeyPath); Assert.AreEqual(42, (int)thirdgen[2].Value);
            Assert.AreEqual("elem1.[0].vstring", thirdgen[3].KeyPath); Assert.AreEqual("hoge", (string)thirdgen[3].Value);
            Assert.AreEqual("elem1.[0].vtrue", thirdgen[4].KeyPath); Assert.AreEqual(true, (bool)thirdgen[4].Value);
        }

        [TestMethod()]
        public void JsonObjectAccessor_GetJsonValueTest()
        {
            var or = new ObjectRepository();

            {
                var orjo = NewJsonObjectBuilder()
                    .AddJsonValue("vstring", "hoge")
                    .AddJsonValue("vint", 42)
                    .AddJsonValue("vdouble", -123.45)
                    .AddJsonValue("vtrue", true)
                    .AddJsonValue("vfalse", false)
                    .BuildJsonObject();

                or.RegisterObject("__jo", orjo);
            }

            var jo = new JsonObjectBuilder(or, new JsonObjectConfiguration().GetJsonValueConverter())
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
            var jo = NewJsonObjectBuilder()
                .SetOnlyRootValue(false)
                .BuildJsonObject();

            Assert.IsFalse((bool)new JsonObjectAccessor(jo).GetJsonValue(null));
            Assert.IsFalse((bool)new JsonObjectAccessor(jo).GetJsonValue(string.Empty));
        }
    }
}