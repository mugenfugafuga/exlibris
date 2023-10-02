using ExcelDna.Integration;
using Exlibris.Core.Websocket;
using Exlibris.Core.Websocket.Echo;

namespace Exlibris.Functions.Communication.Websocket.Echo
{
    public static partial class WebsocketEchoFunctions
    {
        [ExcelFunction(
        Category = Category,
            Name = Category + "." + nameof(Server),
            Description = "echo server.")]
        public static object Server(
            [ExcelArgument(AllowReference = true, Description = "url")] object url,
            [ExcelArgument(Description = "optional argument.if the argument is invalid, the method will not execute the process.")] object identifier
            )
            => ExlibrisAddin.GetFunctionSupport().NoError(identifier).ErrorValueIfThrown(support =>
            {
                var urlstr = support.NotDateTime(url, nameof(url)).GetValueOrThrow<string>();
                
                return support.ObserveObjectRegistration(
                    () =>
                    {
                        var server = new ExlibrisWebsocketServer(urlstr);
                        server.AddWebSocketService<EchoWebSocketBehavior>("/echo");
                        server.Start();
                        return server;
                    },
                    url, identifier);
            });
    }
}
