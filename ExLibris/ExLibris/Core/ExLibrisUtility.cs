using ExcelDna.Integration;
using System;

namespace ExLibris.Core
{
    static class ExLibrisUtility
    {
        public static bool IsMissingOrError(object obj) => IsExcelMissing(obj) || IsExcelError(obj);
        
        public static bool IsExcelMissing(object obj) => obj is ExcelMissing;

        public static bool IsExcelError(object obj) => obj is ExcelError;

        public static bool IsExcelEmpty(object obj) => obj is ExcelEmpty;


        public static void ThrowIfMissingOrError(object obj, Func<string> name)
        {
            if (IsMissingOrError(obj))
            {
                throw new Exception($"{name()} is missing or error.");
            }
        }

        public static void ThrowIfMissingOrErrorOrEmpty(object obj, Func<string> name)
        {
            if (IsMissingOrError(obj) || IsExcelEmpty(obj))
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

        public static object RunAsync(
            string callerFunctionName,
            Func<object> objectFunction,
            params object[] parameters)
            => ExcelAsyncUtil.Run(
                callerFunctionName,
                parameters,
                () => FuncOrNAIfThrown(() =>objectFunction())
                );

        public static object NullIfEmpty(object value) => IsExcelEmpty(value) ? null : value;

        public static object ToExcelValue(object value) => value == null ? ExcelEmpty.Value : value;

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

        public static ExcelValueConverter GetExcelValueConverter(ExLibrisConfiguration configuration)
            => new ExcelValueConverter(configuration.ExcelValueConfiguration);
    }
}
