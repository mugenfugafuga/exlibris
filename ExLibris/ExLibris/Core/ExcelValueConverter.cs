﻿using ExcelDna.Integration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ExLibris.Core
{
    using Pred = Func<object, bool>;

    public class ExcelValueConverter
    {
        private static readonly Pred IsExcelMissing = ExLibrisUtility.IsExcelMissing;
        private static readonly Pred IsExcelError = ExLibrisUtility.IsExcelError;
        private static readonly Pred IsExcelEmpty = ExLibrisUtility.IsExcelEmpty;

        private static readonly Pred IsNull = o => o == null;
        private static readonly Pred IsStringEmpty = o => o is string oo && string.IsNullOrEmpty(oo);

        private readonly List<Pred> ExcelValueToErrorPreds = new List<Pred>();
        private readonly List<Pred> ExcelValueToNullPreds = new List<Pred>();
        private readonly List<Pred> ExcelValueToStringEmptyPreds = new List<Pred>();

        private readonly List<Pred> ToExcelEmptyPreds = new List<Pred>();
        private readonly List<Pred> ToExcelStringEmptyPreds = new List<Pred>();


        public ExcelValueConverter(ExcelValueConfiguration configuration)
        {
            AddPred(configuration.ExcelMissingTo, IsExcelMissing);
            AddPred(configuration.ExcelErrorTo, IsExcelError);
            AddPred(configuration.ExcelEmptyTo, IsExcelEmpty);
            AddPred(configuration.ExcelStringEmptyTo, IsStringEmpty);

            AddPred(configuration.NullToExcel, IsNull);
            AddPred(configuration.StringEmptyToExcel, IsStringEmpty);
        }

        public object ToValue(object excelValue)
        {
            if (ExcelValueToErrorPreds.Any(pred => pred(excelValue)))
            {
                throw new Exception($"value is invalid. value : {excelValue}");
            }

            if (ExcelValueToNullPreds.Any(pred => pred(excelValue)))
            {
                return null;
            }

            if (ExcelValueToStringEmptyPreds.Any(pred => pred(excelValue)))
            {
                return string.Empty;
            }

            return excelValue;
        }

        public object ToExcel(object value)
        {
            if (ToExcelEmptyPreds.Any(pred => pred(value)))
            {
                return ExcelEmpty.Value;
            }

            if (ToExcelStringEmptyPreds.Any(pred => pred(value)))
            {
                return string.Empty;
            }

            return value;
        }

        private void AddPred(ExcelValueTo excelValueTo, Pred pred)
        {
            switch (excelValueTo)
            {
                case ExcelValueTo.Error:
                    ExcelValueToErrorPreds.Add(pred);
                    break;
                case ExcelValueTo.Null:
                    ExcelValueToNullPreds.Add(pred);
                    break;
                case ExcelValueTo.StringEmpty:
                    ExcelValueToStringEmptyPreds.Add(pred);
                    break;
                default:
                    throw new ArgumentException($"unsupported type {excelValueTo}");
            }
        }

        private void AddPred(ToExcelValue toExcelValue, Pred pred)
        {
            switch (toExcelValue)
            {
                case ToExcelValue.ExcelEmpty:
                    ToExcelEmptyPreds.Add(pred);
                    break;
                case ToExcelValue.StringEmpty:
                    ToExcelStringEmptyPreds.Add(pred);
                    break;
                default:
                    throw new ArgumentException($"unsupported type {toExcelValue}");
            }
        }
    }
}
