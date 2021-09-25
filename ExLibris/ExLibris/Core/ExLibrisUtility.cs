using ExcelDna.Integration;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
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

        public static object ExcelObserveObjectRegistration<T>(
               string callerFunctionName,
               string objectName,
               ObjectRepository objectRepository,
               Func<T> func,
               params object[] paramObjects)
               => ExcelObserve(
                   callerFunctionName,
                   () => NewObservableObjectRegistrationHandle<T>(objectName, objectRepository, func),
                   paramObjects
                   );

        public static object ExcelObserveObjectRegistration<T>(
               string callerFunctionName,
               ObjectRepository objectRepository,
               Func<T> func,
               params object[] paramObjects)
               => ExcelObserve(
                   callerFunctionName,
                   () => NewObservableObjectRegistrationHandle<T>(objectRepository, func),
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


        public static IObjectRegistrationHandle NewObservableObjectRegistrationHandle<T>(ObjectRepository objectRepository, Func<T> objectFunc)
            => new ObservableObjectRegistrationHandle<T>(objectRepository, objectFunc);

        public static IObjectRegistrationHandle NewObservableObjectRegistrationHandle<T>(string objectName, ObjectRepository objectRepository, Func<T> objectFunc)
            => new ObservableObjectRegistrationHandle<T>(objectName, objectRepository, objectFunc);

        public static IExcelObservable AggreateExcelObservables(IEnumerable<IExcelObservable> excelObservables, object value)
            => new ExcelObservableAggregation(value, excelObservables);

        private class ObservableObjectRegistrationHandle<T> : IObjectRegistrationHandle, IDisposable
        {
            private ConcurrentBag<IExcelObserver> observers = new ConcurrentBag<IExcelObserver>();
            private readonly ObjectRegistrationHandle<T> registrationHandle;

            public string HandleKey => registrationHandle.HandleKey;

            public ObservableObjectRegistrationHandle(ObjectRepository objectRepository, Func<T> objectFunc)
            {
                this.registrationHandle = new ObjectRegistrationHandle<T>(objectRepository, objectFunc);
            }

            public ObservableObjectRegistrationHandle(string objectName, ObjectRepository objectRepository, Func<T> objectFunc)
            {
                this.registrationHandle = new ObjectRegistrationHandle<T>(objectName, objectRepository, objectFunc);
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

        private class ExcelObservableAggregation : IExcelObservable, IDisposable
        {
            private object value;
            private IEnumerable<IExcelObservable> excelObservables;

            private List<IDisposable> disposables = new List<IDisposable>();

            public ExcelObservableAggregation(object value, IEnumerable<IExcelObservable> excelObservables)
            {
                this.value = value;
                this.excelObservables = excelObservables;
            }

            public void Dispose()
            {
                foreach(var d in disposables)
                {
                    d.Dispose();
                }
            }

            public IDisposable Subscribe(IExcelObserver observer)
            {
                try
                {
                    var es = excelObservables.Select(e => e.Subscribe(observer));
                    disposables.AddRange(es);

                    observer.OnNext(value);

                }
                catch (Exception)
                {
                    observer.OnNext(ExcelError.ExcelErrorNA);
                }

                return this;
            }
        }

        private class PeriodicObservationHandle : IExcelObservable, IDisposable
        {
            private readonly ConcurrentBag<IExcelObserver> observers = new ConcurrentBag<IExcelObserver>();
            private readonly Func<object> func;
            private readonly Timer timer;
            private object current = null;
            private object locker = new object();

            public PeriodicObservationHandle(Func<object> func, int periodMilliSec)
            {
                this.func = func;
                this.timer = new Timer(TimerCallback, null, 0, periodMilliSec);
            }

            public void Dispose()
            {
                timer.Dispose();
            }

            public IDisposable Subscribe(IExcelObserver observer)
            {
                observers.Add(observer);
                return this;
            }

            private void TimerCallback(object _)
            {
                var o = func();

                lock (locker)
                {
                    if (current != null && current.Equals(o))
                    {
                        return;
                    }

                    current = o;
                }

                foreach (var obs in observers)
                {
                    obs.OnNext(o);
                }
            }
        }
    }
}
