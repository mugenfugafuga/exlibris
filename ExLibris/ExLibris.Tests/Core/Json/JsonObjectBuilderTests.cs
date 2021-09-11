using Microsoft.VisualStudio.TestTools.UnitTesting;
using ExLibris.Core.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExLibris.Core.Json.Tests
{
    [TestClass()]
    public class JsonObjectBuilderTests
    {
        [TestMethod()]
        public void JsonObjectBuilder_SetOnlyRootValue_String_Text()
        {
            var jo = new JsonObjectBuilder()
                .SetOnlyRootValue("test")
                .BuildDynamic();


            Assert.AreEqual(@"test", (string)jo);
            Assert.AreEqual(@"""test""", JsonObjectSerialiser.Serialize(jo));
        }

        [TestMethod()]
        public void JsonObjectBuilder_SetOnlyRootValue_Int_Text()
        {
            var jo = new JsonObjectBuilder()
                .SetOnlyRootValue(42)
                .BuildDynamic();


            Assert.AreEqual(42, (int)jo);
            Assert.AreEqual(42, (double)jo);
            Assert.AreEqual(@"42", JsonObjectSerialiser.Serialize(jo));
        }

        [TestMethod()]
        public void JsonObjectBuilder_SetOnlyRootValue_Double_Text()
        {
            var jo = new JsonObjectBuilder()
                .SetOnlyRootValue(-123.45)
                .BuildDynamic();


            Assert.AreEqual(-123.45, (double)jo);
            Assert.AreEqual(@"-123.45", JsonObjectSerialiser.Serialize(jo));
        }

        [TestMethod()]
        public void JsonObjectBuilder_SetOnlyRootValue_True_Text()
        {
            var jo = new JsonObjectBuilder()
                .SetOnlyRootValue(true)
                .BuildDynamic();


            Assert.AreEqual(true, (bool)jo);
            Assert.AreEqual(@"true", JsonObjectSerialiser.Serialize(jo));
        }

        [TestMethod()]
        public void JsonObjectBuilder_SetOnlyRootValue_False_Text()
        {
            var jo = new JsonObjectBuilder()
                .SetOnlyRootValue(false)
                .BuildDynamic();


            Assert.AreEqual(false, (bool)jo);
            Assert.AreEqual(@"false", JsonObjectSerialiser.Serialize(jo));
        }

        [TestMethod()]
        public void JsonObjectBuilder_SetOnlyRootValue_Null_Text()
        {
            var jo = new JsonObjectBuilder()
                .SetOnlyRootValue(null)
                .BuildDynamic();


            Assert.AreEqual(null, jo);
            Assert.AreEqual(@"null", JsonObjectSerialiser.Serialize(jo));
        }

        [TestMethod()]
        [ExpectedException(typeof(Exception))]
        public void JsonObjectBuilder_SetOnlyRootValue_Twice_Throw_Text()
        {
            var jo = new JsonObjectBuilder()
                .SetOnlyRootValue(1)
                .SetOnlyRootValue(2);
        }

        [TestMethod()]
        public void JsonObjectBuilder_BuildJsonObject_1layer_Dict_Text()
        {
            var jo = new JsonObjectBuilder()
                .AddJsonValue("vstring", "hoge")
                .AddJsonValue("vint", 42)
                .AddJsonValue("vdouble", -123.45)
                .AddJsonValue("vtrue", true)
                .AddJsonValue("vfalse", false)
                .BuildDynamic();


            Assert.AreEqual("hoge", (string)jo["vstring"]);
            Assert.AreEqual(42, (int)jo["vint"]);
            Assert.AreEqual(-123.45, (double)jo["vdouble"]);
            Assert.AreEqual(true, (bool)jo["vtrue"]);
            Assert.AreEqual(false, (bool)jo["vfalse"]);
            Assert.AreEqual(
                @"{""vstring"":""hoge"",""vint"":42,""vdouble"":-123.45,""vtrue"":true,""vfalse"":false}",
                JsonObjectSerialiser.Serialize(jo));
        }

        [TestMethod()]
        public void JsonObjectBuilder_BuildJsonObject_1layer_Array_Text()
        {
            var jo = new JsonObjectBuilder()
                .AddJsonValue("[0]", "hoge")
                .AddJsonValue("[1]", 42)
                .AddJsonValue("[2]", -123.45)
                .AddJsonValue("[3]", true)
                .AddJsonValue("[4]", false)
                .BuildDynamic();


            Assert.AreEqual("hoge", (string)jo[0]);
            Assert.AreEqual(42, (int)jo[1]);
            Assert.AreEqual(-123.45, (double)jo[2]);
            Assert.AreEqual(true, (bool)jo[3]);
            Assert.AreEqual(false, (bool)jo[4]);
            Assert.AreEqual(
                @"[""hoge"",42,-123.45,true,false]",
                JsonObjectSerialiser.Serialize(jo));
        }

        [TestMethod()]
        public void JsonObjectBuilder_BuildJsonObject_2layer_Dict_Text()
        {
            var jo = new JsonObjectBuilder()
                .AddJsonValue("vstring", "hoge")
                .AddJsonValue("vint", 42)
                .AddJsonValue("vdouble", -123.45)
                .AddJsonValue("vtrue", true)
                .AddJsonValue("vfalse", false)
                .AddJsonValue("elm0.vstring", "hoge")
                .AddJsonValue("elm0.vint", 42)
                .AddJsonValue("elm1.vdouble", -123.45)
                .AddJsonValue("elm1.vtrue", true)
                .AddJsonValue("elm2.vfalse", false)
                .AddJsonValue("elm3.[0]", "hoge")
                .AddJsonValue("elm3.[1]", 42)
                .AddJsonValue("elm3.[2]", -123.45)
                .AddJsonValue("elm3.[3]", true)
                .AddJsonValue("elm3.[4]", false)
                .BuildDynamic();


            Assert.AreEqual("hoge", (string)jo["vstring"]);
            Assert.AreEqual(42, (int)jo["vint"]);
            Assert.AreEqual(-123.45, (double)jo["vdouble"]);
            Assert.AreEqual(true, (bool)jo["vtrue"]);
            Assert.AreEqual(false, (bool)jo["vfalse"]);
            Assert.AreEqual("hoge", (string)jo["elm0"]["vstring"]);
            Assert.AreEqual(42, (int)jo["elm0"]["vint"]);
            Assert.AreEqual(-123.45, (double)jo["elm1"]["vdouble"]);
            Assert.AreEqual(true, (bool)jo["elm1"]["vtrue"]);
            Assert.AreEqual(false, (bool)jo["elm2"]["vfalse"]);
            Assert.AreEqual("hoge", (string)jo["elm3"][0]);
            Assert.AreEqual(42, (int)jo["elm3"][1]);
            Assert.AreEqual(-123.45, (double)jo["elm3"][2]);
            Assert.AreEqual(true, (bool)jo["elm3"][3]);
            Assert.AreEqual(false, (bool)jo["elm3"][4]);
            Assert.AreEqual(
                @"{""vstring"":""hoge"",""vint"":42,""vdouble"":-123.45,""vtrue"":true,""vfalse"":false,""elm0"":{""vstring"":""hoge"",""vint"":42},""elm1"":{""vdouble"":-123.45,""vtrue"":true},""elm2"":{""vfalse"":false},""elm3"":[""hoge"",42,-123.45,true,false]}",
                JsonObjectSerialiser.Serialize(jo));
        }

        [TestMethod()]
        public void JsonObjectBuilder_BuildJsonObject_2layer_Array_Text()
        {
            var jo = new JsonObjectBuilder()
                .AddJsonValue("[0].vstring", "hoge")
                .AddJsonValue("[0].vint", 42)
                .AddJsonValue("[1].vdouble", -123.45)
                .AddJsonValue("[1].vtrue", true)
                .AddJsonValue("[2].vfalse", false)
                .AddJsonValue("[3].[0]", "hoge")
                .AddJsonValue("[3].[1]", 42)
                .AddJsonValue("[3].[2]", -123.45)
                .AddJsonValue("[3].[3]", true)
                .AddJsonValue("[3].[4]", false)
                .BuildDynamic();


            Assert.AreEqual("hoge", (string)jo[0]["vstring"]);
            Assert.AreEqual(42, (int)jo[0]["vint"]);
            Assert.AreEqual(-123.45, (double)jo[1]["vdouble"]);
            Assert.AreEqual(true, (bool)jo[1]["vtrue"]);
            Assert.AreEqual(false, (bool)jo[2]["vfalse"]);
            Assert.AreEqual("hoge", (string)jo[3][0]);
            Assert.AreEqual(42, (int)jo[3][1]);
            Assert.AreEqual(-123.45, (double)jo[3][2]);
            Assert.AreEqual(true, (bool)jo[3][3]);
            Assert.AreEqual(false, (bool)jo[3][4]);
            Assert.AreEqual(
                @"[{""vstring"":""hoge"",""vint"":42},{""vdouble"":-123.45,""vtrue"":true},{""vfalse"":false},[""hoge"",42,-123.45,true,false]]",
                JsonObjectSerialiser.Serialize(jo));
        }

        [TestMethod()]
        public void JsonObjectBuilder_BuildJsonObject_3layer_Text()
        {
            var jo = new JsonObjectBuilder()
                .AddJsonValue("vstring", "hoge")
                .AddJsonValue("vint", 42)
                .AddJsonValue("vdouble", -123.45)
                .AddJsonValue("vtrue", true)
                .AddJsonValue("vfalse", false)
                .AddJsonValue("elm0.vstring", "hoge")
                .AddJsonValue("elm0.elm00.vint", 42)
                .AddJsonValue("elm1.elm10.vdouble", -123.45)
                .AddJsonValue("elm1.elm10.vtrue", true)
                .AddJsonValue("elm2.vfalse", false)
                .AddJsonValue("elm3.[0]", "hoge")
                .AddJsonValue("elm3.[1].vint", 42)
                .AddJsonValue("elm3.[1].vdouble", -123.45)
                .AddJsonValue("elm3.[2].[0]", true)
                .AddJsonValue("elm3.[2].[1]", false)
                .BuildDynamic();


            Assert.AreEqual("hoge", (string)jo["vstring"]);
            Assert.AreEqual(42, (int)jo["vint"]);
            Assert.AreEqual(-123.45, (double)jo["vdouble"]);
            Assert.AreEqual(true, (bool)jo["vtrue"]);
            Assert.AreEqual(false, (bool)jo["vfalse"]);
            Assert.AreEqual("hoge", (string)jo["elm0"]["vstring"]);
            Assert.AreEqual(42, (int)jo["elm0"]["elm00"]["vint"]);
            Assert.AreEqual(-123.45, (double)jo["elm1"]["elm10"]["vdouble"]);
            Assert.AreEqual(true, (bool)jo["elm1"]["elm10"]["vtrue"]);
            Assert.AreEqual(false, (bool)jo["elm2"]["vfalse"]);
            Assert.AreEqual("hoge", (string)jo["elm3"][0]);
            Assert.AreEqual(42, (int)jo["elm3"][1]["vint"]);
            Assert.AreEqual(-123.45, (double)jo["elm3"][1]["vdouble"]);
            Assert.AreEqual(true, (bool)jo["elm3"][2][0]);
            Assert.AreEqual(false, (bool)jo["elm3"][2][1]);
            Assert.AreEqual(
                @"{""vstring"":""hoge"",""vint"":42,""vdouble"":-123.45,""vtrue"":true,""vfalse"":false,""elm0"":{""vstring"":""hoge"",""elm00"":{""vint"":42}},""elm1"":{""elm10"":{""vdouble"":-123.45,""vtrue"":true}},""elm2"":{""vfalse"":false},""elm3"":[""hoge"",{""vint"":42,""vdouble"":-123.45},[true,false]]}",
                JsonObjectSerialiser.Serialize(jo));
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidCastException))]
        public void JsonObjectBuilder_BuildJsonObject_DifferentObjectType_Throw_Text()
        {
            var jo = new JsonObjectBuilder()
                .AddJsonValue("elem0.elem1", 1)
                .AddJsonValue("[0].elem1", 2);
        }

        [TestMethod()]
        public void JsonObjectBuilder_BuildJsonObject_UsingObjectRepository_Text()
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

            var jo0 = new JsonObjectBuilder(or)
                .SetOnlyRootValue("__jo")
                .BuildJsonObject();
            Assert.AreEqual(
                @"{""vstring"":""hoge"",""vint"":42,""vdouble"":-123.45,""vtrue"":true,""vfalse"":false}",
                JsonObjectSerialiser.Serialize(jo0));

            var jo1 = new JsonObjectBuilder(or)
                .AddJsonValue("elem0", "__jo")
                .AddJsonValue("elem1.[0]", "__jo")
                .AddJsonValue("elem2", 42)
                .BuildJsonObject();
            Assert.AreEqual(
                @"{""elem0"":{""vstring"":""hoge"",""vint"":42,""vdouble"":-123.45,""vtrue"":true,""vfalse"":false},""elem1"":[{""vstring"":""hoge"",""vint"":42,""vdouble"":-123.45,""vtrue"":true,""vfalse"":false}],""elem2"":42}",
                JsonObjectSerialiser.Serialize(jo1));
        }
    }
}