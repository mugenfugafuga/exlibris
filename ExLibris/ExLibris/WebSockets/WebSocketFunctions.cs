using ExcelDna.Integration;
using ExLibris.Core;
using ExLibris.Core.WebSockets;
using System;

namespace ExLibris.WebSockets
{
    public static class WebSocketFunctions
    {
        [ExcelFunction(
            Name = "ExLibris.WebSockets.OpenWebSocket",
            Category = "ExLibris.WebSockets")]
        public static object OpenWebSocket(string webSocketUri, object identifier)
        {
            if (ExLibrisUtility.IsExcelError(identifier))
            {
                return ExcelError.ExcelErrorNA;
            }

            var context = ExLibrisContext.DefaultContext;
            var configuration = context.DefaultExLibrisConfiguration;

            return ExLibrisUtility.ObserveObjectHandle(
                nameof(OpenWebSocket),
                context.ObjectRepository,
                () => new WebsocketClient(webSocketUri),
                webSocketUri,
                identifier
                );
        }

        [ExcelFunction(
            Name = "ExLibris.WebSockets.ObserveWebSocketStatus",
            Category = "ExLibris.WebSockets")]
        public static object ObserveWebSocketStatus(string webSocketHandle, int periodMilliSec = 10000)
        {
            var context = ExLibrisContext.DefaultContext;
            var client = context.ObjectRepository.GetObject<WebsocketClient>(webSocketHandle);

            return ExLibrisUtility.ObserveObjectPeriodically(
                nameof(ObserveWebSocketStatus),
                () => client.WebSocket.State.ToString(),
                periodMilliSec,
                webSocketHandle, 
                periodMilliSec
                );
        }

        [ExcelFunction(
            Name = "ExLibris.WebSockets.ObserveWebSocketMessage",
            Category = "ExLibris.WebSockets")]
        public static object ObserveWebSocketMessage(string webSocketHandle)
        {
            var context = ExLibrisContext.DefaultContext;
            var client = context.ObjectRepository.GetObject<WebsocketClient>(webSocketHandle);

            return WebSocketUtility.ObserveWebSocketMessage(
                nameof(ObserveWebSocketMessage),
                client,
                webSocketHandle
                );
        }

        [ExcelFunction(
            Name = "ExLibris.WebSockets.SendWebSocketMessage",
            Category = "ExLibris.WebSockets")]
        public static object SendWebSocketMessage(string webSocketHandle, string message, object afterThis)
        {
            if (ExLibrisUtility.IsExcelError(afterThis))
            {
                return ExcelError.ExcelErrorNA;
            }

            if(afterThis is bool && !(bool)afterThis)
            {
                return ExcelError.ExcelErrorNA;
            }

            var context = ExLibrisContext.DefaultContext;

            return ExLibrisUtility.FuncOrNAIfThrown(() =>
            {
                var client = context.ObjectRepository.GetObject<WebsocketClient>(webSocketHandle);
                client.SendMessage(message);

                return $"message sent : {DateTime.Now.ToString("yyyy-MM-ddThh:mm:ss.fff")}";
            });

        }
    }
}
