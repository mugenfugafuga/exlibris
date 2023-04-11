using ExcelDna.Integration;

namespace Exlibris.Functions.Utility;

partial class UtilityFunctions
{
    [ExcelFunction(
        Category = Category,
        Name = $"{Category}.{nameof(TimeAtInterval)}",
        Description = "display the current time at regular intervals")]
    public static object TimeAtInterval(
        [ExcelArgument(Description = "interval time (sec). default value : 10")] object interval,
        [ExcelArgument(Description = "type of time. 0 : double (default), 1 : string")] object timeType,
        [ExcelArgument(Description = "optional argument.if the argument is invalid, the method will not execute the process.")] object identifier)
        => ExlibrisAddin.GetFunctionSupport().NoError(identifier).ErrorValueIfThrown(support =>
        {
            var intervalms = support.NotDateTime(interval, nameof(interval)).ShouldBeScalar().GetValueOrDefault(10) * 1000;
            var timetype = support.NotDateTime(timeType, nameof(timeType)).ShouldBeScalar().GetValueOrDefault(DisplayTimeType.Double);
            var timefunc = GetCurrentTimeFunc(timetype);

            return support.Observe((observer, disposer) =>
            {
                var timer = new System.Timers.Timer
                {
                    Interval = intervalms,
                };
                timer.Elapsed += (_0, _1) =>
                {
                    observer.OnNext(timefunc());
                };

                disposer.Add(timer).Start();
                observer.OnNext(timefunc());
            }, interval, timeType, identifier);
        });
}
