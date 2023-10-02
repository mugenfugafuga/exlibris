using WebSocketSharp.Server;

namespace Exlibris.Core.Websocket
{
    public class ExlibrisWebsocketServer : AbstractDisposable
    {
        private readonly WebSocketServer server;

        public ExlibrisWebsocketServer (string url)
        {
            this.server = new WebSocketServer (url);
        }

        public ExlibrisWebsocketServer AddWebSocketService<T>(string path) where T : WebSocketBehavior, new()
        {
            server.AddWebSocketService<T>(path);
            return this;
        }

        public ExlibrisWebsocketServer Start()
        {
            server.Start ();
            return this;
        }

        public override void OnDisposing()
        {
            server.Stop ();
        }
    }
}
