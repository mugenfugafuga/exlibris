using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Exlibris.Core.JSONs.Tests;

[TestClass()]
public class JSONUtilTests
{
    [TestMethod()]
    public void SplitPathTest1()
    {
        var keys = JSONUtil.SplitPath("$.hoge.fuga.'higa.hega[3]'.hoga").ToList();

        Assert.AreEqual(5, keys.Count);
        Assert.IsTrue(keys[0].IsRoot); Assert.IsFalse(keys[0].IsArrayItem); Assert.AreEqual("$", keys[0].Name); Assert.AreEqual(-1, keys[0].ArrayIndex); Assert.AreEqual("$", keys[0].ToString()); Assert.IsTrue(keys[0].HasNext);
        Assert.IsFalse(keys[1].IsRoot); Assert.IsFalse(keys[1].IsArrayItem); Assert.AreEqual("hoge", keys[1].Name); Assert.AreEqual(-1, keys[1].ArrayIndex); Assert.AreEqual("hoge", keys[1].ToString()); Assert.IsTrue(keys[1].HasNext);
        Assert.IsFalse(keys[2].IsRoot); Assert.IsFalse(keys[2].IsArrayItem); Assert.AreEqual("fuga", keys[2].Name); Assert.AreEqual(-1, keys[2].ArrayIndex); Assert.AreEqual("fuga", keys[2].ToString()); Assert.IsTrue(keys[2].HasNext);
        Assert.IsFalse(keys[3].IsRoot); Assert.IsTrue(keys[3].IsArrayItem); Assert.AreEqual("higa.hega", keys[3].Name); Assert.AreEqual(3, keys[3].ArrayIndex); Assert.AreEqual("higa.hega[3]", keys[3].ToString()); Assert.IsTrue(keys[3].HasNext);
        Assert.IsFalse(keys[4].IsRoot); Assert.IsFalse(keys[4].IsArrayItem); Assert.AreEqual("hoga", keys[4].Name); Assert.AreEqual(-1, keys[4].ArrayIndex); Assert.AreEqual("hoga", keys[4].ToString()); Assert.IsFalse(keys[4].HasNext);
    }

    [TestMethod()]
    public void SplitPathTest2()
    {
        var keys = JSONUtil.SplitPath("$.hoge.fuga.'higa.hega[3]'").ToList();

        Assert.AreEqual(4, keys.Count);
        Assert.IsTrue(keys[0].IsRoot); Assert.IsFalse(keys[0].IsArrayItem); Assert.AreEqual("$", keys[0].Name); Assert.AreEqual(-1, keys[0].ArrayIndex); Assert.AreEqual("$", keys[0].ToString()); Assert.IsTrue(keys[0].HasNext);
        Assert.IsFalse(keys[1].IsRoot); Assert.IsFalse(keys[1].IsArrayItem); Assert.AreEqual("hoge", keys[1].Name); Assert.AreEqual(-1, keys[1].ArrayIndex); Assert.AreEqual("hoge", keys[1].ToString()); Assert.IsTrue(keys[1].HasNext);
        Assert.IsFalse(keys[2].IsRoot); Assert.IsFalse(keys[2].IsArrayItem); Assert.AreEqual("fuga", keys[2].Name); Assert.AreEqual(-1, keys[2].ArrayIndex); Assert.AreEqual("fuga", keys[2].ToString()); Assert.IsTrue(keys[2].HasNext);
        Assert.IsFalse(keys[3].IsRoot); Assert.IsTrue(keys[3].IsArrayItem); Assert.AreEqual("higa.hega", keys[3].Name); Assert.AreEqual(3, keys[3].ArrayIndex); Assert.AreEqual("higa.hega[3]", keys[3].ToString()); Assert.IsFalse(keys[3].HasNext);
    }

    [TestMethod()]
    public void SplitPathTest3()
    {
        var keys = JSONUtil.SplitPath("$").ToList();

        Assert.AreEqual(1, keys.Count);
        Assert.IsTrue(keys[0].IsRoot); Assert.IsFalse(keys[0].IsArrayItem); Assert.AreEqual("$", keys[0].Name); Assert.AreEqual(-1, keys[0].ArrayIndex); Assert.AreEqual("$", keys[0].ToString()); Assert.IsFalse(keys[0].HasNext);
    }

    [TestMethod()]
    public void SplitPathTest4()
    {
        var keys = JSONUtil.SplitPath("$[5]").ToList();

        Assert.AreEqual(1, keys.Count);
        Assert.IsTrue(keys[0].IsRoot); Assert.IsTrue(keys[0].IsArrayItem); Assert.AreEqual("$", keys[0].Name); Assert.AreEqual(5, keys[0].ArrayIndex); Assert.AreEqual("$[5]", keys[0].ToString()); Assert.IsFalse(keys[0].HasNext);
    }
}