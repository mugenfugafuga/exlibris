using ExcelDna.Integration;
using Exlibris.Core.UDP;

namespace Exlibris.Functions.Communication.UDP
{
    public static partial class UDPFunctions
    {
        [ExcelFunction(
            Category = Category,
            Name = Category + "." + nameof(MulticastReceiver),
            Description = "generates udp multicast receiver.")]
        public static object MulticastReceiver(
            [ExcelArgument(AllowReference = true, Description = "multicast IP address")] object multicastAddress,
            [ExcelArgument(AllowReference = true, Description = "multicast Port")] object multicastPort,
            [ExcelArgument(Description = "optional argument.if the argument is invalid, the method will not execute the process.")] object identifier
            )
            => ExlibrisAddin.GetFunctionSupport().NoError(identifier).ErrorValueIfThrown(support =>
            {
                var mip = support.NotDateTime(multicastAddress, nameof(multicastAddress)).GetValueOrThrow<string>();
                var port = support.NotDateTime(multicastPort, nameof(multicastPort)).GetValueOrThrow<int>();

                return support.ObserveObjectRegistration(
                    () => new MulticastReceiver(mip, port),
                    multicastAddress, multicastPort, identifier);
            });
    }
}
