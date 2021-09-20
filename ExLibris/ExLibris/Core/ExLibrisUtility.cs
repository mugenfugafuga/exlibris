using ExcelDna.Integration;
using System;

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

        public static IExcelObservable NewExcelObservableDoNothingOnDisposing(object value) => new ExcelObservableDoNothingOnDisposing(value);

        public static object RunAsync(
            string callerFunctionName,
            Func<object> objectFunction,
            params object[] parameters)
            => ExcelAsyncUtil.Run(
                callerFunctionName,
                parameters,
                () => FuncOrNAIfThrown(() =>objectFunction())
                );

        public static IExcelObservable FuncOrObjservableNAIfThrown<T>(Func<T> func) where T : IExcelObservable
        {
            try
            {
                return func();
            }
            catch (Exception)
            {
                return NewExcelObservableDoNothingOnDisposing(ExcelError.ExcelErrorNA);
            }
        }

        class ExcelObservableDoNothingOnDisposing : IExcelObservable, IDisposable
        {
            private object value;

            public ExcelObservableDoNothingOnDisposing(object value)
            {
                this.value = value;
            }

            public void Dispose()
            {
            }

            public IDisposable Subscribe(IExcelObserver observer)
            {
                observer.OnNext(value);
                return this;
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
                   () => NewObjectRegistrationHandle<T>(objectName, objectRepository, func),
                   paramObjects
                   );

        public static object ObserveObjectHandle<T>(
               string callerFunctionName,
               ObjectRepository objectRepository,
               Func<T> func,
               params object[] paramObjects)
               => ExcelObserve(
                   callerFunctionName,
                   () => NewObjectRegistrationHandle<T>(objectRepository, func),
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
                       () => FuncOrObjservableNAIfThrown(() => new PeriodicExeCutionHandle(func, periodMilliSec))
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


        public static IExcelObservable NewObjectRegistrationHandle<T>(ObjectRepository objectRepository, Func<T> objectFunc)
    => new ObjectRegistrationHandle<T>(objectRepository, objectFunc);

        public static IExcelObservable NewObjectRegistrationHandle<T>(string objectName, ObjectRepository objectRepository, Func<T> objectFunc)
            => new ObjectRegistrationHandle<T>(objectName, objectRepository, objectFunc);

        private class ObjectRegistrationHandle<T> : IExcelObservable, IDisposable
        {
            private static readonly Action doNothing = () => { };

            public readonly string HandleKey;
            public readonly ObjectRepository objectRepository;

            public T Value { get; private set; }


            private readonly Func<T> objectFunc;


            public ObjectRegistrationHandle(ObjectRepository objectRepository, Func<T> objectFunc) :
                this(typeof(T).FullName, objectRepository, objectFunc)
            {
            }

            public ObjectRegistrationHandle(string objectName, ObjectRepository objectRepository, Func<T> objectFunc)
            {
                HandleKey = $"{objectName}:{Guid.NewGuid().ToString().ToUpper()}";
                this.objectRepository = objectRepository;
                this.objectFunc = objectFunc;
            }

            public virtual void Dispose()
            {
                objectRepository.Remove(HandleKey);

                if (Value != null && Value is IDisposable)
                {
                    ((IDisposable)Value).Dispose();
                }
            }

            public IDisposable Subscribe(IExcelObserver observer)
            {
                try
                {
                    var v = objectFunc();
                    Value = v;
                    objectRepository.RegisterObject(HandleKey, v);

                    observer.OnNext(HandleKey);
                }
                catch (Exception)
                {
                    observer.OnNext(ExcelError.ExcelErrorNA);
                }
                return this;
            }
        }
    }
}
