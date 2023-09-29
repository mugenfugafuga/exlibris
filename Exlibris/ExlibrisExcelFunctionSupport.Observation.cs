using ExcelDna.Integration;
using Exlibris.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Exlibris
{
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
        () => new ExcelObservableImpl(this, (observer, disposer) =>
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
            () => new ExcelObservableImpl(this, action));

        public object Observe(
            Func<IExcelObserver, CompositeDisposable, CancellationToken, Task> taskSource,
            params object[] callerParameters)
        => ExcelAsyncUtil.Observe(
            ExcelFunctionName,
            DecompositeCallerParameters(callerParameters).ToArray(),
            () => new TaskExcelObservableImpl(this, taskSource));

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
            private readonly ExlibrisExcelFunctionSupport support;
            private readonly Action<IExcelObserver, CompositeDisposable> action;

            public ExcelObservableImpl(ExlibrisExcelFunctionSupport support, Action<IExcelObserver, CompositeDisposable> action)
            {
                this.support = support;
                this.action = action;
            }

            public IDisposable Subscribe(IExcelObserver observer)
            {
                var disposer = new CompositeDisposable();
                var obs = new ExcelObserverImpl(observer, disposer, support);

                try
                {
                    action(obs, disposer);
                    return disposer;
                }
                catch (Exception ex)
                {
                    obs.OnError(ex);
                    obs.OnCompleted();
                    disposer.Dispose();
                    return DoNothingOnDisposing.Instance;
                }
            }
        }

        private class TaskExcelObservableImpl : IExcelObservable
        {
            private readonly ExlibrisExcelFunctionSupport support;
            private readonly Func<IExcelObserver, CompositeDisposable, CancellationToken, Task> taskSource;

            public TaskExcelObservableImpl(ExlibrisExcelFunctionSupport support, Func<IExcelObserver, CompositeDisposable, CancellationToken, Task> taskSource)
            {
                this.support = support;
                this.taskSource = taskSource;
            }

            public IDisposable Subscribe(IExcelObserver observer)
            {
                var disposer = new CompositeDisposable();
                var obs = new ExcelObserverImpl(observer, disposer, support);
                var cancellationTokenSource = support.CacheDisposable(new CancellationTokenSource(), disposer);

                try
                {
                    var task = support.CacheDisposable(taskSource(obs, disposer, cancellationTokenSource.Token), disposer);

                    task.ContinueWith(t =>
                    {
                        if (t.Exception != null)
                        {
                            obs.OnError(t.Exception);
                        }
                    });

                    return disposer;
                }
                catch (Exception ex)
                {
                    obs.OnError(ex);
                    obs.OnCompleted();
                    disposer.Dispose();
                    return DoNothingOnDisposing.Instance;
                }
            }
        }

        private class ExcelObserverImpl : IExcelObserver
        {
            private const long notCompleted = 0;
            private const long completed = 1;

            private long status = notCompleted;

            private readonly IExcelObserver observer;
            private readonly CompositeDisposable disposer;
            private readonly ExlibrisExcelFunctionSupport support;
            private readonly Action onCompletedOnce;

            public ExcelObserverImpl(IExcelObserver observer, CompositeDisposable disposer, ExlibrisExcelFunctionSupport support)
            {
                this.observer = observer;
                this.disposer = disposer;
                this.support = support;
                onCompletedOnce = CallOnce.New(() => observer.OnCompleted());
            }

            public void OnCompleted()
            {
                Interlocked.Exchange(ref status, completed);
                onCompletedOnce();
            }

            public void OnNext(object value)
            {
                if (Interlocked.Read(ref status) == notCompleted)
                {
                    observer.OnNext(support.ToExcel(value, disposer));
                }
            }

            public void OnError(Exception exception)
            {
                if (Interlocked.CompareExchange(ref status, completed, notCompleted) == notCompleted)
                {
                    support.Cache(exception);
                    observer.OnNext(support.ToExcel(ExcelError.ExcelErrorValue, disposer));
                }
            }
        }
    }
}