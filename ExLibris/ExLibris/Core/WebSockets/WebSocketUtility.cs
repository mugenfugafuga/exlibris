using ExcelDna.Integration;

namespace ExLibris.Core.WebSockets
{
    static class WebSocketUtility
    {
        public static object ExcelObserveWebSocketMessage(
            string callerFunctionName,
            WebsocketClient client,
            params object[] paramObjects)
               => ExcelAsyncUtil.Observe(
                       callerFunctionName,
                       paramObjects,
                       () => new WebSocketMessageReveiverHandle(client)
                       );
    }
}
