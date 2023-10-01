using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Exlibris.Core.UDP
{
    public class MulticastReceiver : AbstractDisposable, IReceiver
    {
        private readonly IPAddress multicast;
        private readonly UdpClient client;
        public MulticastReceiver(string multicastAddress, int multicastPort)
        {
            MulticastAddress = multicastAddress;
            MulticastPort = multicastPort;

            multicast = IPAddress.Parse(multicastAddress);
            client = new UdpClient();
            client.JoinMulticastGroup(multicast);
            client.ExclusiveAddressUse = false;
            client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

            var ep = new IPEndPoint(IPAddress.Any, multicastPort);
            client.Client.Bind(ep);
        }

        public override void OnDisposing()
        {
            client.Dispose();
        }

        public async Task<Message> ReceiveAsync()
        {
            var recdt = DateTime.Now;
            var result = await client.ReceiveAsync();
            var buf = (byte[]) result.Buffer.Clone();

            try
            {
                var stmessage = Encoding.ASCII.GetString(buf);

                return new Message
                {
                    DateTime = recdt,
                    PayloadType = PayloadType.String,
                    RemoteEndPoint = result.RemoteEndPoint,
                    Payload = stmessage,
                };
            }
            catch
            {
                return new Message
                {
                    DateTime = recdt,
                    PayloadType = PayloadType.Byte,
                    RemoteEndPoint = result.RemoteEndPoint,
                    Payload = buf,
                };
            }
        }

        public string MulticastAddress { get; }

        public int MulticastPort { get; }
    }
}
