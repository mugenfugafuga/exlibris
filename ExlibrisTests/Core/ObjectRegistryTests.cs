using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Exlibris.Core.Tests
{
    [TestClass()]
    public class ObjectRegistryTests
    {
        [TestMethod()]
        public void Basic_Test()
        {
            var or = new ConcurrentObjectRegistry();

            var val = new JValue(123);

            var handle = or.RegisterObject(val);

            Assert.IsTrue(handle.Key.StartsWith($"{val.GetType()}:"));

            Assert.AreEqual(val, handle.Value);
            Assert.IsTrue(handle.ValueOrThrow<JValue>() != null);
            Assert.IsTrue(handle.ValueOrThrow<JToken>() != null);
            Assert.AreEqual((long)123, handle.ValueOrThrow<JValue>().Value);
            Assert.ThrowsException<InvalidCastException>(() => handle.ValueOrThrow<JObject>());

            var key = handle.Key;

            Assert.AreEqual(val, or.GetObject(key));
            Assert.IsTrue(or.GetObjectOrThrow<JValue>(key) != null);
            Assert.IsTrue(or.GetObjectOrThrow<JToken>(key) != null);
            Assert.AreEqual((long)123, or.GetObjectOrThrow<JValue>(key).Value);
            Assert.ThrowsException<InvalidCastException>(() => or.GetObjectOrThrow<JObject>(key));
            Assert.AreEqual(handle, or.GetObjectHandleOrThrow(key));
        }

        [TestMethod()]
        public void Register_Null_Object_Test()
        {
            var or = new ConcurrentObjectRegistry();
#pragma warning disable CS8625
            Assert.ThrowsException<NullReferenceException>(() => or.RegisterObject(null));
#pragma warning restore CS8625
        }

        [TestMethod()]
        public void Key_Not_Found_Test()
        {
            var or = new ConcurrentObjectRegistry();
            Assert.ThrowsException<KeyNotFoundException>(() => or.GetObjectOrThrow("test"));
            Assert.ThrowsException<KeyNotFoundException>(() => or.GetObjectOrThrow<int>("test"));
            Assert.ThrowsException<KeyNotFoundException>(() => or.GetObjectHandleOrThrow("test"));
        }
    }
}