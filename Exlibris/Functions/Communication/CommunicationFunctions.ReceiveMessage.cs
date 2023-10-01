using ExcelDna.Integration;
using Exlibris.Core;
using System;

namespace Exlibris.Functions.Communication
{
    public static partial class CommunicationFunctions
    {
        [ExcelFunction(
            Category = Category,
            Name = Category + "." + nameof(ReceiveMessage),
            Description = "receive message")]
        public static object ReceiveMessage(
            [ExcelArgument(AllowReference = true, Description = "receiver")] object receiver,
            [ExcelArgument(Description = "optional argument.if the argument is invalid, the method will not execute the process.")] object identifier
            )
            => ExlibrisAddin.GetFunctionSupport().NoError(identifier).ErrorValueIfThrown(support =>
            {
                var rcver = support.NotDateTime(receiver, nameof(receiver)).ShouldBeScalar().GetValueOrThrow<IReceiver>();

                return support.Observe(async (observer, disposer) =>
                {
                    var run = new Atomic<bool>(true);

                    disposer.AddAction(() => run.Value = false);

                    while (run.Value)
                    {
                        try
                        {
                            var message = await rcver.ReceiveAsync();
                            observer.OnNext(support.JSONSerializer.FromObject(message));
                        }
                        catch (Exception ex)
                        {
                            observer.OnError(ex);
                        }
                    }
                }, receiver, identifier);
            });
    }
}
