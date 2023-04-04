using ExcelDna.Integration;
using Exlibris.Configuration;
using Exlibris.Core;
using Exlibris.Core.JSONs;
using Exlibris.Excel;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System.Diagnostics;

namespace Exlibris;

using Pred = Predicate<object?>;

public partial class ExlibrisExcelFunctionSupport
{
    private static ExcelAddress GetCallingCell()
        => ExlibrisUtil.GetAddress(XlCall.Excel(XlCall.xlfCaller) as ExcelReference) ?? throw new NullReferenceException($"can not resolve the calling cell.");

    private static void CheckNoError(params object[] args)
    {
        foreach (var arg in args.SelectMany(a => ResolveValues(a)))
        {
            if (arg is ExcelError)
            {
                throw new ArgumentException($"some arguments are invalid.");
            }
        }
    }

    private static IEnumerable<object> ResolveValues(object value)
    {
        if (value is object[,] m)
        {
            foreach (var v in m) { yield return v; }
        }
        else if (value is object[] a)
        {
            foreach (var v in a) { yield return v; }
        }
        else
        {
            if (value is ExcelReference er)
            {
                yield return er;
                foreach (var o in ResolveValues(er.GetValue()))
                {
                    yield return o;
                }
            }
            else
            {
                yield return value;
            }
        }
    }

    public ExlibrisExcelFunctionSupport(
        Func<ExlibrisConfiguration> configurationFactory,
        IObjectRegistry objectRegistry,
        ThrownExceptions thrownExceptions,
        IJSONSerializer<JObject, JArray, JValue, JToken, JSchema> serializer)
    {
        CallingCell = GetCallingCell();

        configuration = configurationFactory();
        excelValueConverter = new ExcelSingleValueConverter(configuration.ExcelValueConversion, objectRegistry, CallingCell);

        ObjectRegistry = objectRegistry;
        ThrownExceptions = thrownExceptions;
        JSONSerializer = serializer;

        // TODO : A function that calls 'observe' may do this twice. Therefore, do not remove the exception
        ExceptionOfReference = thrownExceptions.ExceptionOf(CallingCell, false);
        DoOnceIfThrown = CallOnce.New<Exception>(e => ExceptionOfReference.Is = e);
    }

    public string? ExcelFunctionName { get; set; }

    public ExcelAddress CallingCell { get; }

    private ExlibrisConfiguration configuration;
    private ExcelSingleValueConverter excelValueConverter;
    public ExlibrisConfiguration ExlibrisConfiguration
    {
        get => configuration;
        set
        {
            configuration = value;
            excelValueConverter = new ExcelSingleValueConverter(value.ExcelValueConversion, ObjectRegistry, CallingCell);
        }
    }

    public IObjectRegistry ObjectRegistry { get; }

    public ThrownExceptions ThrownExceptions { get; }

    public IJSONSerializer<JObject, JArray, JValue, JToken, JSchema> JSONSerializer { get; }

    private ThrownExceptions.ExceptionOfReference ExceptionOfReference { get; }
    
    private Action<Exception> DoOnceIfThrown { get; }

    public ExlibrisExcelFunctionSupport NoError(params object[] arguments)
    {
        try
        {
            CheckNoError(arguments);

            return this;
        }
        catch (Exception ex)
        {
            ex.Data.Add("calling-function", ExcelFunctionName);
            DoOnceIfThrown(ex);
            throw;
        }
    }

    public object ErrorValueIfThrown<T>(Func<ExlibrisExcelFunctionSupport, T> func)
    {
        Debug.Assert(ExcelFunctionName != null);

        try
        {
            return ToExcel(func(this), null) ?? throw new NullReferenceException();
        }
        catch (Exception ex)
        {
            ex.Data.Add("calling-function", ExcelFunctionName);
            DoOnceIfThrown(ex);
            throw;
        }
    }

    public Exception? GetThrownException(ExcelAddress address) => ThrownExceptions[address];

    public IObjectHandle RegisterObject(object obj) => ObjectRegistry.RegisterObject(obj, CallingCell);

    private class ExcelSingleValueConverter
    {
        private static readonly Pred IsExcelError = o => o is ExcelError;
        private static readonly Pred IsExcelMissing = o => o is ExcelMissing;
        private static readonly Pred IsExcelEmpty = o => o is ExcelEmpty;
        private static readonly Pred IsStringEmpty = o => o is string oo && string.IsNullOrEmpty(oo);
        private static readonly Pred IsNull = o => o is null;

        private readonly List<Pred> ExcelToExcelErrorPreds = new();
        private readonly List<Pred> ExcelToNullPreds = new();
        private readonly List<Pred> ExcelToStringEmptyPreds = new();

        private readonly List<Pred> ValueToExcelEmptyPreds = new();
        private readonly List<Pred> ValueToStringEmptyPreds = new();

        private readonly IObjectRegistry objectRegistry;
        private readonly ExcelAddress? callingCell;

        public ExcelSingleValueConverter(ExcelValueConversionConfiguration configuration, IObjectRegistry objectRegistry, ExcelAddress? callingCell)
        {
            AddPred(configuration.ExcelErrorTo, IsExcelError);
            AddPred(configuration.ExcelMissingTo, IsExcelMissing);
            AddPred(configuration.ExcelEmptyTo, IsExcelEmpty);
            AddPred(configuration.ExcelStringEmptyTo, IsStringEmpty);

            AddPred(configuration.NullTo, IsNull);
            AddPred(configuration.StringEmptyTo, IsStringEmpty);

            this.objectRegistry = objectRegistry;
            this.callingCell = callingCell;
        }

        private void AddPred(ObjectType type, Pred pred)
        {
            switch (type)
            {
                case ObjectType.ExcelError: ExcelToExcelErrorPreds.Add(pred); break;
                case ObjectType.Null: ExcelToNullPreds.Add(pred); break;
                case ObjectType.StringEmpty: ExcelToStringEmptyPreds.Add(pred); break;
                default: throw new NotImplementedException(type.ToString());
            }
        }

        private void AddPred(ExcelValueType type, Pred pred)
        {
            switch (type)
            {
                case ExcelValueType.ExcelEmpty: ValueToExcelEmptyPreds.Add(pred); break;
                case ExcelValueType.StringEmpty: ValueToStringEmptyPreds.Add(pred); break;
                default: throw new NotImplementedException(type.ToString());
            }
        }

        public object? FromExcel(object excel, ExcelAddress? address, DateTimeDetector? dateTimeDetector)
        {
            if (ExcelToExcelErrorPreds.Any(pred => pred(excel)))
            {
                if (address == null)
                {
                    throw new Exception($"value is invalid. value : {excel}");
                }
                else
                {
                    throw new Exception($"value is invalid. value : {excel}, address : {address?.Address}");
                }
            }

            if (ExcelToNullPreds.Any(pred => pred(excel)))
            {
                return null;
            }

            if (ExcelToStringEmptyPreds.Any(pred => pred(excel)))
            {
                return string.Empty;
            }

            if (excel is double dd)
            {
                if (dateTimeDetector?.IsDateTime(address) ?? false)
                {
                    return DateTime.FromOADate(dd);
                }

                var ii = (int)dd; if (ii == dd) { return ii; }
                var ll = (long)dd; if (ll == dd) { return ll; }
                return dd;
            }

            if (excel is string ss)
            {
                return objectRegistry.GetObject(ss) ?? ss;
            }

            return excel;
        }

        public object ToExcel(object? value, CompositeDisposable? disposer)
        {
            if (ValueToExcelEmptyPreds.Any(pred => pred(value)))
            {
                return ExcelEmpty.Value;
            }

            if (ValueToStringEmptyPreds.Any(pred => pred(value)))
            {
                return string.Empty;
            }

            if (value == null || IsReturnValuesOfType(value))
            {
                return value ?? ExcelError.ExcelErrorNum;
            }

            return disposer?.Add(objectRegistry.RegisterObject(value, callingCell)).Key ??
                throw new InvalidOperationException("disposer is null.");
        }

        // https://excel-dna.net/reference-data-type-marshalling/
        private static bool IsReturnValuesOfType(object? value)
            => value is 
                double or
                string or
                DateTime or
                bool or
                ExcelError or
                ExcelMissing or
                ExcelEmpty or
                ExcelReference or
                int or
                short or
                ushort or
                decimal or
                long;
    }
}
