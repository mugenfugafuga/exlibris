using ExcelDna.Integration;

namespace ExLibris.Core.WebSockets
{
    static class WebSocketUtility
    {
        public static object ExcelObserveWebSocketMessage(
            string collerFunctionName,
            WebsocketClient client,
            params object[] paramObjects)
               => ExcelAsyncUtil.Observe(
                       collerFunctionName,
                       paramObjects,
                       () => new WebSocketMessageReveiverHandle(client)
                       );
    }
}
