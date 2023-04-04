using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Exlibris.Core.Tests;

[TestClass()]
public class CallOnceTests
{
    [TestMethod()]
    public void NewTest()
    {
        var called = 0;

        var action = CallOnce.New(() => called++);
        Parallel.For(0, 10, (_) => action());

        Assert.AreEqual(1, called);
    }

    [TestMethod()]
    public void NewTest_OneArg()
    {
        var called = 0;

        var action = CallOnce.New<int>((arg) => called += arg);
        Parallel.For(0, 10, (_) => action(2));

        Assert.AreEqual(2, called);
    }
}