using Microsoft.VisualStudio.TestTools.UnitTesting;
using Exlibris.Core.UDP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exlibris.Core.UDP.Tests;

[TestClass()]
public class MulticastSenderReceiverTests
{
    [TestMethod()]
    public void MulticastTest()
    {
        var mip = "239.0.0.1";
        var port = 10018;

        var sender = new MulticastSender(mip, port);
        var reciever = new MulticastReceiver(mip, port);

        try
        {
            var rec = reciever.ReceiveAsync();

            sender.Send("hello, multicast.");

            Assert.AreEqual("hello, multicast.", rec.Result.StringPayload);
        }
        finally
        {
            reciever.Dispose();
            sender.Dispose();
        }
    }
}