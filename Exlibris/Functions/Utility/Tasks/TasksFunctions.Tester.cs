using ExcelDna.Integration;

namespace Exlibris.Functions.Utility.Tasks;
partial class TasksFunctions
{
    [ExcelFunction(
        Category = Category,
        Name = $"{Category}.{nameof(Tester)}",
        Description = "display the current time at regular intervals",
        IsHidden = true)]
    public static object Tester(
        [ExcelArgument(AllowReference = true, Description = "optional argument. maxTimes. default : 1000")] object maxTimes,
        [ExcelArgument(AllowReference = true, Description = "optional argument. Specifies the number of attempts until throwing a exception. : 1000")] object throwOnAttempt,
        [ExcelArgument(Description = "optional argument.if the argument is invalid, the method will not execute the process.")] object identifier)
        => ExlibrisAddin.GetFunctionSupport().ErrorValueIfThrown(support =>
        {
            var max = support.NotDateTime(maxTimes, nameof(maxTimes)).ShouldBeScalar().GetValueOrDefault(1000);
            var toa = support.NotDateTime(throwOnAttempt, nameof(throwOnAttempt)).ShouldBeScalar().GetValueOrDefault(1000);

            return support.Observe((observer, disposer, cancellationToken) => Task.Run(async () =>
            {
                var counter = 0;

                observer.OnNext($"{counter++}-{DateTime.Now}");

                while (true)
                {
                    await Task.Delay(1000);
                    observer.OnNext($"{counter++}-{DateTime.Now}");

                    cancellationToken.ThrowIfCancellationRequested();

                    if (counter >= max) { break; }
                    if (counter >= toa) { throw new Exception("something just happened."); }
                }

                observer.OnNext($"finished:{counter++}-{DateTime.Now}");

            }, cancellationToken), maxTimes, throwOnAttempt, identifier);
        });
}
