using ExcelDna.Integration;
using Exlibris.Core;

namespace Exlibris;
partial class ExlibrisExcelFunctionSupport
{
    private static IEnumerable<object> DecompositeCallerParameters(object[] callerParameters)
    {
        foreach (var para in callerParameters)
        {
            if (para is ExcelReference er)
            {
                yield return er;
                yield return er.GetValue();
            }
            else
            {
                yield return para;
            }
        }
    }

    public object Observe<T>(
    bool observationCompleted,
    Func<IExcelObserver, CompositeDisposable, T> func,
    params object[] callerParameters)
=> ExcelAsyncUtil.Observe(
    ExcelFunctionName,
    DecompositeCallerParameters(callerParameters).ToArray(),
    () => new ExcelObservableImpl(DoOnceIfThrown, ToExcel, (observer, disposer) =>
    {
        observer.OnNext(func(observer, disposer));
        if (observationCompleted) { observer.OnCompleted(); }
    }));

    public object Observe(
        Action<IExcelObserver, CompositeDisposable> action,
        params object[] callerParameters)
    => ExcelAsyncUtil.Observe(
        ExcelFunctionName,
        DecompositeCallerParameters(callerParameters).ToArray(),
        () => new ExcelObservableImpl(DoOnceIfThrown, ToExcel, action));

    public object ObserveObjectRegistration(
        Func<object> generation,
        params object[] callerParameters)
    => Observe((IExcelObserver observer, CompositeDisposable disposer) =>
    {
        var val = generation();
        var handle = disposer.Add(RegisterObject(val));
        observer.OnNext(handle.Key);
    }, callerParameters);

    private class ExcelObservableImpl : IExcelObservable
    {
        private readonly Action<Exception> doIfThrown;
        private readonly Func<object?, CompositeDisposable?, object> toExcel;
        private readonly Action<IExcelObserver, CompositeDisposable> action;

        public ExcelObservableImpl(Action<Exception> doIfThrown, Func<object?, CompositeDisposable?, object> toExcel, Action<IExcelObserver, CompositeDisposable> action)
        {
            this.doIfThrown = doIfThrown;
            this.toExcel = toExcel;
            this.action = action;
        }

        public IDisposable Subscribe(IExcelObserver observer)
        {
            var obs = new ExcelObserverImpl(observer, doIfThrown, toExcel);
            var disposables = obs.Disposer;

            try
            {
                action(obs, disposables);
                return disposables;
            }
            catch (Exception ex)
            {
                obs.OnError(ex);
                obs.OnCompleted();
                disposables.Dispose();
                return DoNothingOnDisposing.Instance;
            }
        }
    }

    private class ExcelObserverImpl : IExcelObserver
    {
        private readonly IExcelObserver observer;
        private readonly Action<Exception> doIfThrown;
        private readonly Func<object?, CompositeDisposable?, object> toExcel;
        private readonly Action onCompletedOnce;

        public CompositeDisposable Disposer { get; } = new CompositeDisposable();
        
        public ExcelObserverImpl(IExcelObserver observer, Action<Exception> doIfThrown, Func<object?, CompositeDisposable?, object> toExcel)
        {
            this.observer = observer;
            this.doIfThrown = doIfThrown;
            this.toExcel = toExcel;
            onCompletedOnce = CallOnce.New(() => observer.OnCompleted());
        }

        public void OnCompleted() => onCompletedOnce();

        public void OnError(Exception exception)
        {
            doIfThrown(exception);
            observer.OnError(exception);
        }

        public void OnNext(object value) => observer.OnNext(toExcel(value, Disposer));
    }
}
