using ExcelDna.Integration;
using System;

namespace ExLibris.Core
{
    static class ExcelDnaUtility
    {
        public static bool IsMissingOrError(object obj) => obj is ExcelMissing || obj is ExcelError;
        public static bool IsEmpty(object obj) => obj is ExcelEmpty;

        public static void ThrowIfMissingOrError(object obj, Func<string> name)
        {
            if (IsMissingOrError(obj))
            {
                throw new Exception($"{name()} is missing or error.");
            }
        }

        public static void ThrowIfMissingOrErrorOrEmpty(object obj, Func<string> name)
        {
            if (IsMissingOrError(obj) || IsEmpty(obj))
            {
                throw new Exception($"{name()} is missing or error or empty.");
            }
        }


        public static void ThrowIfMissingOrError(object obj, string name)
        {
            if(IsMissingOrError(obj))
            {
                throw new Exception($"{name} is missing or error.");
            }
        }

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

        public static object ObserveExcelObservableDoNothingOnDisposing(
            string callerFunctionName,
            Func<object> objectFunction,
            params object[] parameters)
            => ExcelAsyncUtil.Observe(
                callerFunctionName,
                parameters,
                () => FuncOrNAIfThrown(() => NewExcelObservableDoNothingOnDisposing(objectFunction()))
                );

        public static object NullIfEmpty(object value) => IsEmpty(value) ? null : value;

        public static object ToExcelValue(object value) => value == null ? ExcelEmpty.Value : value;

        public static IExcelObservable FuncOrNAIfThrown(Func<IExcelObservable> func)
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

        public static IExcelObservable FuncOrNAIfThrown(Func<ObjectHandle> func)
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
    }
}
