using ExcelDna.Integration;
using Exlibris.Core;
using System;

namespace Exlibris.Functions.Communication
{
    public static partial class CommunicationFunctions
    {
        [ExcelFunction(
        Category = Category,
            Name = Category + "." + nameof(SendMessage),
            Description = "send message")]
        public static object SendMessage(
            [ExcelArgument(AllowReference = true, Description = "sender")] object sender,
            [ExcelArgument(AllowReference = true, Description = "message")] object message,
            [ExcelArgument(Description = "optional argument.if the argument is invalid, the method will not execute the process.")] object identifier
            )
            => ExlibrisAddin.GetFunctionSupport().NoError(identifier).ErrorValueIfThrown(support =>
            {
                var snder = support.NotDateTime(sender, nameof(sender)).GetValueOrThrow<ISender>();
                var msg = support.NotDateTime(message, nameof(message)).GetValueOrThrow<string>();

                snder.Send(msg);
                return $"Sent Message. {DateTime.Now}";
            });
    }
}
