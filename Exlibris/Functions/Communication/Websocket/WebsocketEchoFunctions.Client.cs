using ExcelDna.Integration;
using Exlibris.Core.Websocket;

namespace Exlibris.Functions.Communication.Websocket
{
    public static partial class WebsocketEchoFunctions
    {
        [ExcelFunction(
        Category = Category,
            Name = Category + "." + nameof(Client),
            Description = "websocket client.")]
        public static object Client(
            [ExcelArgument(AllowReference = true, Description = "url")] object url,
            [ExcelArgument(Description = "optional argument.if the argument is invalid, the method will not execute the process.")] object identifier
            )
            => ExlibrisAddin.GetFunctionSupport().NoError(identifier).ErrorValueIfThrown(support =>
            {
                var urlstr = support.NotDateTime(url, nameof(url)).GetValueOrThrow<string>();

                return support.ObserveObjectRegistration(
                    () => new ExlibrisWebsocketClient(urlstr),
                    url, identifier);
            });
    }
}
