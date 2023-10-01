using System;
using System.Runtime.Serialization;

namespace Exlibris.Core
{
    [DataContract(Name = "message")]
    public class Message
    {
        [DataMember(Name = "payload_type", Order = 10)]
        public PayloadType PayloadType { get; set; }

        [DataMember(Name = "date_time", Order = 20)]
        public DateTime DateTime { get; set; }

        [DataMember(Name = "remote_end_point", Order = 30)]
        public string RemoteEndPoint { get; set; }


        [DataMember(Name = "payload", Order = 110)]
        public object Payload { get; set; }

        [IgnoreDataMember]
        public string StringPayload => (string)Payload;

        [IgnoreDataMember]
        public byte[] BytesPayload => (byte[])Payload;
    }
}
