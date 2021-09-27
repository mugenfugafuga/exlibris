using ExcelDna.Integration;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ExLibris.Core
{
    static class ExLibrisUtility
    {
        public static bool IsExcelMissing(object obj) => obj is ExcelMissing;

        public static bool IsExcelError(object obj) => obj is ExcelError;

        public static bool IsExcelEmpty(object obj) => obj is ExcelEmpty;

        public static object FuncOrNAIfThrown<T>(Func<T> func)
        {
            try
            {
                return func();
            }
            catch (Exception)
            {
                return ExcelError.ExcelErrorNA;
            }
        }

        public static object RunAsync(
            string callerFunctionName,
            Func<object> objectFunction,
            params object[] parameters)
            => ExcelAsyncUtil.Run(
                callerFunctionName,
                parameters,
                () => FuncOrNAIfThrown(() => objectFunction())
                );

        public static object ExcelObserve(
            string callerFunctionName,
            Func<IExcelObservable> func,
            params object[] paramObjects
            )
            => ExcelAsyncUtil.Observe(
                       callerFunctionName,
                       paramObjects,
                       () => func()
                       );

        public static object ExcelObserveObjectAsync<T>(
            string callerFunctionName,
            Func<T> func,
            params object[] paramObjects
            )
            => ExcelObserve(
                callerFunctionName,
                () => NewObservableObjectHandleAsync<T>(func),
                paramObjects
                );


        public static object ExcelObserveObjectRegistrationAsync<T>(
               string callerFunctionName,
               string objectName,
               ObjectRepository objectRepository,
               Func<T> func,
               params object[] paramObjects)
               => ExcelObserve(
                   callerFunctionName,
                   () => NewObservableObjectRegistrationHandleAsync<T>(objectName, objectRepository, func),
                   paramObjects
                   );

        public static object ExcelObserveObjectRegistrationAsync<T>(
               string callerFunctionName,
               ObjectRepository objectRepository,
               Func<T> func,
               params object[] paramObjects)
               => ExcelObserve(
                   callerFunctionName,
                   () => NewObservableObjectRegistrationHandleAsync<T>(objectRepository, func),
                   paramObjects
                   );

        public static object ExcelObserveObjectPeriodically(
               string callerFunctionName,
               Func<object> func,
               int periodMilliSec,
               params object[] paramObjects)
               => ExcelAsyncUtil.Observe(
                       callerFunctionName,
                       paramObjects,
                       () => new PeriodicObservationHandle(func, periodMilliSec)
                       );

        public static IExcelObservable NewObservableObjectHandleAsync<T>(Func<T> func) => new ObservableObjectHandleAsync<T>(func);

        public static IExcelObservable NewObservableObjectRegistrationHandleAsync<T>(ObjectRegistrationHandle<T> objectRegistrationHandle)
            => new ObservableObjectRegistrationHandleAsync<T>(objectRegistrationHandle);

        public static IExcelObservable NewObservableObjectRegistrationHandleAsync<T>(ObjectRepository objectRepository, Func<T> objectFunc)
            => NewObservableObjectRegistrationHandleAsync(new ObjectRegistrationHandle<T>(objectRepository, objectFunc));

        public static IExcelObservable NewObservableObjectRegistrationHandleAsync<T>(string objectName, ObjectRepository objectRepository, Func<T> objectFunc)
            => NewObservableObjectRegistrationHandleAsync(new ObjectRegistrationHandle<T>(objectName, objectRepository, objectFunc));

        public static IExcelObservable NewObservableDisposableObjectAsync<T>(Func<(T Value, IEnumerable<IDisposable> Disposables)> generaitor)
            => new ObservableDisposableObjectAsync<T>(generaitor);

        private abstract class AbstractObservableObjectHandle<T> : IExcelObservable, IDisposable
        {
            public T Value { get; private set; }

            private readonly Func<T> objectFunc;

            public AbstractObservableObjectHandle(Func<T> objectFunc)
            {
                this.objectFunc = objectFunc;
            }

            public void Dispose()
            {
                if (Value != null && Value is IDisposable)
                {
                    ((IDisposable)Value).Dispose();
                }
            }

            public void Update(IExcelObserver observer)
            {
                try
                {
                    var v = objectFunc();
                    Value = v;

                    observer.OnNext(v);
                }
                catch (Exception)
                {
                    observer.OnNext(ExcelError.ExcelErrorNA);
                }
            }

            public abstract IDisposable Subscribe(IExcelObserver observer);
        }

        private class ObservableObjectHandleAsync<T> : AbstractObservableObjectHandle<T>
        {
            public ObservableObjectHandleAsync(Func<T> objectFunc) : base(objectFunc)
            {
            }

            public override IDisposable Subscribe(IExcelObserver observer)
            {
                Task.Run(() => Update(observer));
                return this;
            }
        }

        private abstract class AbstractObservableObjectRegistrationHandle<T> : IExcelObservable, IDisposable
        {
            private readonly ObjectRegistrationHandle<T> registrationHandle;

            public string HandleKey => registrationHandle.HandleKey;

            public AbstractObservableObjectRegistrationHandle(ObjectRegistrationHandle<T> registrationHandle)
            {
                this.registrationHandle = registrationHandle;
            }
            public virtual void Dispose()
            {
                registrationHandle.Dispose();
            }

            public void Update(IExcelObserver observer)
            {
                try
                {
                    registrationHandle.CallRegistration();
                    observer.OnNext(HandleKey);
                }
                catch (Exception)
                {
                    observer.OnNext(ExcelError.ExcelErrorNA);
                }
            }
            public abstract IDisposable Subscribe(IExcelObserver observer);
        }

        private class ObservableObjectRegistrationHandleAsync<T> : AbstractObservableObjectRegistrationHandle<T>
        {
            public ObservableObjectRegistrationHandleAsync(ObjectRegistrationHandle<T> registrationHandle) : base(registrationHandle)
            {
            }

            public override IDisposable Subscribe(IExcelObserver observer)
            {
                Task.Run(() => Update(observer));
                return this;
            }
        }

        private class PeriodicObservationHandle : IExcelObservable, IDisposable
        {
            private readonly Func<object> func;
            private readonly Timer timer;
            private readonly int periodMilliSec;
            private readonly object locker = new object();
            private IExcelObserver observer;
            private object current = null;

            public PeriodicObservationHandle(Func<object> func, int periodMilliSec)
            {
                this.func = func;
                this.periodMilliSec = periodMilliSec;
                this.timer = new Timer(TimerCallback, null, Timeout.Infinite, Timeout.Infinite);
            }

            public void Dispose()
            {
                timer.Change(Timeout.Infinite, Timeout.Infinite);
                timer.Dispose();
            }

            public IDisposable Subscribe(IExcelObserver observer)
            {
                this.observer = observer;

                TimerCallback(null);
                timer.Change(0, periodMilliSec);
                return this;
            }

            private void TimerCallback(object _)
            {
                try
                {
                    lock (locker)
                    {
                        var o = func();

                        if (current != null && current.Equals(o))
                        {
                            return;
                        }

                        current = o;
                        observer.OnNext(o);
                    }
                }
                catch (Exception)
                {
                    // ignore exception
                }
            }
        }

        private class ObservableDisposableObjectAsync<T> : IExcelObservable, IDisposable
        {
            private Func<(T Value, IEnumerable<IDisposable> Disposables)> generator;
            private T value;
            private IEnumerable<IDisposable> disposables;

            public ObservableDisposableObjectAsync(Func<(T Value, IEnumerable<IDisposable> Disposables)> generaitor)
            {
                this.generator = generaitor;
            }

            public void Dispose()
            {
                if (disposables != null)
                {
                    foreach (var d in disposables)
                    {
                        d.Dispose();
                    }
                }

                if (value != null && value is IDisposable)
                {
                    ((IDisposable)value).Dispose();
                }
            }

            public IDisposable Subscribe(IExcelObserver observer)
            {
                Task.Run(() => Update(observer));
                return this;
            }

            private void Update(IExcelObserver observer)
            {
                try
                {
                    (value, disposables) = generator();
                    observer.OnNext(value);
                }
                catch (Exception)
                {
                    observer.OnNext(ExcelError.ExcelErrorNA);
                }
            }
        }
    }
}
