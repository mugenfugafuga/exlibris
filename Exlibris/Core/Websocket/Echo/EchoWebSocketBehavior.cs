using WebSocketSharp;
using WebSocketSharp.Server;

namespace Exlibris.Core.Websocket.Echo
{
    internal class EchoWebSocketBehavior : WebSocketBehavior
    {
        protected override void OnMessage(MessageEventArgs e)
        {
            Send($"{e.Data} - server.");
        }
    }
}
