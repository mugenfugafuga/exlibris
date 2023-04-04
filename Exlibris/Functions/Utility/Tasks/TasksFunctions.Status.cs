using ExcelDna.Integration;

namespace Exlibris.Functions.Utility.Tasks;
partial class TasksFunctions
{
    [ExcelFunction(
    Category = Category,
    Name = $"{Category}.{nameof(Status)}",
    Description = "show the task status")]
    public static object Status(
        [ExcelArgument(AllowReference = true, Description = "target cell")] object targetCell,
        [ExcelArgument(Description = "interval time (sec). default value : 10")] object interval,
        [ExcelArgument(Description = "optional argument.if the argument is invalid, the method will not execute the process.")] object identifier)
        => ExlibrisAddin.GetFunctionSupport().NoError(identifier).ErrorValueIfThrown(support =>
        {
            var address = ExlibrisUtil.GetRawExcel(targetCell, nameof(targetCell)).Address ?? throw new ArgumentException($"{nameof(targetCell)} is not cell.");
            var intervalms = support.NotDateTime(interval, nameof(interval)).ShouldBeScalar().GetValueOrDefault(10) * 1000;

            return support.Observe((observer, disposer) =>
            {
                var action = () =>
                {
                    try
                    {
                        var status = support.GetCachedObject<Task>(address).Status;
                        observer.OnNext($"{status}({(int)status})");
                    }
                    catch (Exception ex)
                    {
                        observer.OnError(ex);
                        observer.OnCompleted();
                    }
                };

                var timer = new System.Timers.Timer
                {
                    Interval = intervalms,
                };

                timer.Elapsed += (_0, _1) =>
                {
                    action();
                };

                disposer.Add(timer).Start();
                action();
            }, targetCell, interval, identifier);
        });
}
