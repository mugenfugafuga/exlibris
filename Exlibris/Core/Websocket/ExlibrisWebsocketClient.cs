using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using WebSocketSharp;

namespace Exlibris.Core.Websocket
{
    public class ExlibrisWebsocketClient : AbstractDisposable, IReceiver, ISender
    {
        private readonly WebSocket socket;
        private readonly ConcurrentQueue<Message> messages = new ConcurrentQueue<Message>();

        public ExlibrisWebsocketClient(string url)
        {
            socket = new WebSocket(url);

            socket.OnMessage += (sender, e) =>
            {
                var dt = DateTime.Now;
                if (e.IsPing) { return; }

                var msg = new Message
                {
                    DateTime = dt,
                    RemoteEndPoint = "unknown",
                    PayloadType = PayloadType.String,
                    Payload = e.Data,
                };

                messages.Enqueue(msg);
            };

            socket.Connect();
        }

        public override void OnDisposing()
        {
            socket.Close();
        }

        public async Task<Message> ReceiveAsync()
        {
            Message msg;
            while (!messages.TryDequeue(out msg))
            {
                await Task.Delay(1);
            }
            return msg;
        }

        public void Send(string message)
        {
            socket.Send(message);
        }
    }
}
