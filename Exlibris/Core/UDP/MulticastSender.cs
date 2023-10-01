using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Exlibris.Core.UDP
{
    public class MulticastSender : AbstractDisposable, ISender
    {
        private readonly IPAddress multicast;
        private readonly UdpClient client;
        public MulticastSender(string multicastAddress, int multicastPort)
        {
            MulticastAddress = multicastAddress;
            MulticastPort = multicastPort;

            multicast = IPAddress.Parse(multicastAddress);
            client = new UdpClient();
            client.JoinMulticastGroup(multicast);
        }

        public void Send(string message)
        {
            var data = Encoding.ASCII.GetBytes(message);
            client.Send(data, data.Length, MulticastAddress, MulticastPort);
        }

        public override void OnDisposing()
        {
            client.Dispose();
        }

        public string MulticastAddress { get; }

        public int MulticastPort { get; }
    }
}
