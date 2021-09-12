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

        public static ExcelValueConverter GetExcelValueConverter(ExLibrisConfiguration configuration)
            => new ExcelValueConverter(configuration.ExcelValueConfiguration);
    }
}
