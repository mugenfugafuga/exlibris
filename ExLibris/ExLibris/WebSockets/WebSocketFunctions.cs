using ExcelDna.Integration;
using ExLibris.Core;
using ExLibris.Core.WebSockets;
using System;

namespace ExLibris.WebSockets
{
    public static class WebSocketFunctions
    {
        private const string categoryName = "ExLibris.WebSockets";
        private const string prefixFunctionName = categoryName + ".";

        [ExcelFunction(
            Name = prefixFunctionName + nameof(OpenWebSocketAsync),
            Category = categoryName)]
        public static object OpenWebSocketAsync(string webSocketUri, string subProtocol, object identifier)
        {
            var context = ExLibrisContext.DefaultContext;
            var support = context.GetFunctionCallSupport();

            return ExLibrisUtility.ExcelObserveObjectRegistrationAsync(
                nameof(OpenWebSocketAsync),
                support.ObjectRepository,
                () =>
                {
                    if (ExLibrisUtility.IsExcelError(identifier))
                    {
                        throw new ArgumentException($"{nameof(identifier)} is Error");
                    }

                    if (string.IsNullOrWhiteSpace(subProtocol))
                    {
                        return new WebsocketClient(webSocketUri);
                    }
                    else
                    {
                        return new WebsocketClient(webSocketUri, subProtocol);
                    }
                },
                webSocketUri,
                subProtocol,
                identifier
                );
        }

        [ExcelFunction(
            Name = prefixFunctionName + nameof(ObserveWebSocketStatus),
            Category = categoryName)]
        public static object ObserveWebSocketStatus(string webSocketHandle, int periodMilliSec = 10000)
        {
            var context = ExLibrisContext.DefaultContext;
            var support = context.GetFunctionCallSupport();

            return ExLibrisUtility.FuncOrNAIfThrown(() =>
            {
                var client = support.ObjectRepository.GetObject<WebsocketClient>(webSocketHandle);

                return ExLibrisUtility.ExcelObserveObjectPeriodically(
                    nameof(ObserveWebSocketStatus),
                    () => client.WebSocket.State.ToString(),
                    periodMilliSec,
                    webSocketHandle,
                    periodMilliSec
                    );
            });
        }

        [ExcelFunction(
            Name = prefixFunctionName + nameof(ObserveWebSocketMessage),
            Category = categoryName)]
        public static object ObserveWebSocketMessage(string webSocketHandle)
        {
            var context = ExLibrisContext.DefaultContext;
            var support = context.GetFunctionCallSupport();

            return ExLibrisUtility.FuncOrNAIfThrown(() =>
            {
                var client = support.ObjectRepository.GetObject<WebsocketClient>(webSocketHandle);

                return WebSocketUtility.ExcelObserveWebSocketMessage(
                    nameof(ObserveWebSocketMessage),
                    client,
                    webSocketHandle
                    );
            });
        }

        [ExcelFunction(
            Name = "ExLibris.WebSockets.SendWebSocketMessage",
            Category = "ExLibris.WebSockets")]
        public static object SendWebSocketMessage(string webSocketHandle, string message, object afterThis)
        {
            var context = ExLibrisContext.DefaultContext;
            var support = context.GetFunctionCallSupport();

            return ExLibrisUtility.FuncOrNAIfThrown(() =>
            {
                if (ExLibrisUtility.IsExcelError(afterThis))
                {
                    throw new Exception($"{nameof(afterThis)} is Error");
                }

                if (afterThis is bool at && !at)
                {
                    throw new Exception($"{nameof(afterThis)} is false");
                }

                var client = support.ObjectRepository.GetObject<WebsocketClient>(webSocketHandle);
                client.SendMessage(message);

                return $"message sent : {DateTime.Now:yyyy-MM-ddThh:mm:ss.fff}";
            });

        }
    }
}
