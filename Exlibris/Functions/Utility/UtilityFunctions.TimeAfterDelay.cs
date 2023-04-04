using ExcelDna.Integration;

namespace Exlibris.Functions.Utility;

partial class UtilityFunctions
{
    [ExcelFunction(
        Category = Category,
        Name = $"{Category}.{nameof(TimeAfterDelay)}",
        Description = "display the current time at regular intervals",
        IsHidden = true)]
    public static object TimeAfterDelay(
        [ExcelArgument(Description = "delay (sec).")] object delay,
        [ExcelArgument(Description = "type of time. 0 : double (default), 1 : string")] object timeType,
        [ExcelArgument(Description = "optional argument.if the argument is invalid, the method will not execute the process.")] object identifier)
        => ExlibrisAddin.GetFunctionSupport().NoError(identifier).ErrorValueIfThrown(support =>
        {
            var delayms = support.NotDateTime(delay, nameof(delay)).ShouldBeScalar().GetValueOrThrow<int>() * 1000;
            var timetype = support.NotDateTime(timeType, nameof(timeType)).ShouldBeScalar().GetValueOrDefault(DisplayTimeType.Double);
            var timefunc = GetCurrentTimeFunc(timetype);

            return support.Observe((observer, disposer) =>
            {
                if (delayms <= 0)
                {
                    observer.OnNext(timefunc());
                    observer.OnCompleted();
                    return;
                }

                var timer = new System.Timers.Timer
                {
                    Interval = delayms,
                    AutoReset = false,
                };

                timer.Elapsed += (_0, _1) =>
                {
                    observer.OnNext(timefunc());
                    observer.OnCompleted();
                };

                disposer.Add(timer).Start();
                observer.OnNext(ExcelError.ExcelErrorNA);
            }, delay, timeType, identifier);
        });
}
