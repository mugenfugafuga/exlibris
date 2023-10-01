using System;
using System.Net;

namespace Exlibris.Core
{
    public class Message
    {
        public PayloadType PayloadType { get; set; }

        public DateTime DateTime { get; set; }

        public IPEndPoint RemoteEndPoint { get; set; }


        public object Payload { get; set; }

        public string StringPayload => (string)Payload;

        public byte[] BytesPayload => (byte[])Payload;
    }
}
