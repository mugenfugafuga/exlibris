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
            catch(Exception)
            {
                return ExcelError.ExcelErrorNA;
            }
        }

        public static IExcelObservable NewObservableObjectHandle<T>(Func<T> func) => new ObservableObjectHandle<T>(func);

        public static object RunAsync(
            string callerFunctionName,
            Func<object> objectFunction,
            params object[] parameters)
            => ExcelAsyncUtil.Run(
                callerFunctionName,
                parameters,
                () => FuncOrNAIfThrown(() =>objectFunction())
                );

        private class ObservableObjectHandle<T> : IExcelObservable, IDisposable
        {
            public T Value { get; private set; }


            private readonly Func<T> objectFunc;
            private ConcurrentBag<IExcelObserver> observers = new ConcurrentBag<IExcelObserver>();

            public ObservableObjectHandle(Func<T> objectFunc)
            {
                this.objectFunc = objectFunc;
            }

            public void Dispose()
            {
                if(Value != null && Value is IDisposable)
                {
                    ((IDisposable)Value).Dispose();
                }
            }

            public IDisposable Subscribe(IExcelObserver observer)
            {
                observers.Add(observer);
                Task.Run(Update);
                return this;
            }

            private void Update()
            {
                try
                {
                    var v = objectFunc();
                    Value = v;

                    foreach (var observer in observers)
                    {
                        observer.OnNext(v);
                    }
                }
                catch (Exception)
                {
                    foreach (var observer in observers)
                    {
                        observer.OnNext(ExcelError.ExcelErrorNA);
                    }
                }
            }
        }

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

        public static object ObserveObjectPeriodically(
               string callerFunctionName,
               Func<object> func,
               int periodMilliSec,
               params object[] paramObjects)
               => ExcelAsyncUtil.Observe(
                       callerFunctionName,
                       paramObjects,
                       () => new PeriodicObservationHandle(func, periodMilliSec)
                       );

        public static object ObserveObject<T>(
            string callerFunctionName,
            Func<T> func,
            params object[] paramObjects
            )
            => ExcelObserve(
                callerFunctionName,
                () => NewObservableObjectHandle<T>(func),
                paramObjects
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


        public static IExcelObservable NewObservableObjectRegistrationHandleAsync<T>(ObjectRepository objectRepository, Func<T> objectFunc)
            => new ObservableObjectRegistrationHandleAsync<T>(new ObjectRegistrationHandle<T>(objectRepository, objectFunc));

        public static IExcelObservable NewObservableObjectRegistrationHandleAsync<T>(string objectName, ObjectRepository objectRepository, Func<T> objectFunc)
            => new ObservableObjectRegistrationHandleAsync<T>(new ObjectRegistrationHandle<T>(objectName, objectRepository, objectFunc));

        public static IExcelObservable NewObservableDisposableObject<T>(Func<(T Value, IEnumerable<IDisposable> Disposables)> generaitor)
            => new ObservableDisposableObjectAsync<T>(generaitor);

        private class ObservableObjectRegistrationHandleAsync<T> : IExcelObservable, IDisposable
        {
            private ConcurrentBag<IExcelObserver> observers = new ConcurrentBag<IExcelObserver>();
            private readonly ObjectRegistrationHandle<T> registrationHandle;

            public string HandleKey => registrationHandle.HandleKey;

            public ObservableObjectRegistrationHandleAsync(ObjectRegistrationHandle<T> registrationHandle)
            {
                this.registrationHandle = registrationHandle;
            }
            public virtual void Dispose()
            {
                registrationHandle.Dispose();
            }

            public IDisposable Subscribe(IExcelObserver observer)
            {
                observers.Add(observer);
                Task.Run(Update);
                return this;
            }

            private void Update()
            {
                try
                {
                    registrationHandle.CallRegistration();

                    foreach(var observer in observers)
                    {
                        observer.OnNext(HandleKey);
                    }
                }
                catch (Exception)
                {
                    foreach (var observer in observers)
                    {
                        observer.OnNext(ExcelError.ExcelErrorNA);
                    }
                }
            }
        }

        private class PeriodicObservationHandle : IExcelObservable, IDisposable
        {
            private readonly ConcurrentBag<IExcelObserver> observers = new ConcurrentBag<IExcelObserver>();
            private readonly Func<object> func;
            private readonly Timer timer;
            private readonly int periodMilliSec;
            private object current = null;
            private readonly object locker = new object();

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
                observers.Add(observer);

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

                        foreach (var obs in observers)
                        {
                            obs.OnNext(o);
                        }
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
            private ConcurrentBag<IExcelObserver> observers = new ConcurrentBag<IExcelObserver>();
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
                observers.Add(observer);
                Task.Run(Update);
                return this;
            }

            private void Update()
            {

                try
                {
                    (value, disposables) = generator();

                    foreach (var o in observers)
                    {
                        o.OnNext(value);
                    }
                }
                catch (Exception)
                {
                    foreach (var o in observers)
                    {
                        o.OnNext(ExcelError.ExcelErrorNA);
                    }
                }
            }
        }
    }
}
