using ExcelDna.Integration;

namespace ExLibris.Core.WebSockets
{
    static class WebSocketUtility
    {
        public static object ObserveWebSocketMessage(
            string collerFunctionName,
            WebsocketClient client,
            params object[] paramObjects)
               => ExcelAsyncUtil.Observe(
                       collerFunctionName,
                       paramObjects,
                       () => ExLibrisUtility.FuncOrObjservableNAIfThrown(() => new WebSocketMessageReveiverHandle(client))
                       );
    }
}
