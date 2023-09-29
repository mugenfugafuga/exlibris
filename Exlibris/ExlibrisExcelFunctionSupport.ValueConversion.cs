using ExcelDna.Integration;
using Exlibris.Core;
using Exlibris.Excel;
using System;

namespace Exlibris
{
    partial class ExlibrisExcelFunctionSupport
    {
        public MatrixBuilder NewMatrixBuilder() => new MatrixBuilder();

        public IExcelValue NewExcelValue(object excel, string name, DateTimeDetector dateTimeDetector)
        {
            var reference = excel is ExcelReference er ? er : null;
            var excl = reference?.GetValue() ?? excel;
            var address = ExlibrisUtil.GetAddress(reference);

            if (excl is object[,] matrix)
            {
                return new MatrixExcelValue(address, matrix, excelValueConverter, name, dateTimeDetector);
            }

            if (excl is object[] _)
            {
                throw new NotImplementedException();
            }

            // excl is scalar.
            return new ScalarExcelValue(address, excl, excelValueConverter, name, dateTimeDetector);
        }

        public IExcelValue NotDateTime(object excel, string name) => NewExcelValue(excel, name, null);

        public IScalar ShouldBeScalar(object excel, string name, DateTimeDetector dateTimeDetector)
            => NewExcelValue(excel, name, dateTimeDetector).ShouldBeScalar();

        public IMatrix ShouldBeMatrix(object excel, string name, DateTimeDetector dateTimeDetector)
            => NewExcelValue(excel, name, dateTimeDetector).ShouldBeMatrix();

        private object ToExcel(object value, CompositeDisposable disposer)
        {
            if (value is object[,] m)
            {
                for (var r = 0; r < m.GetLongLength(0); ++r)
                {
                    for (var c = 0; c < m.GetLongLength(1); ++c)
                    {
                        m[r, c] = excelValueConverter.ToExcel(m[r, c], disposer);
                    }
                }
                return m;
            }

            if (value is object[] a)
            {
                for (var c = 0; c < a.GetLongLength(0); ++c)
                {
                    a[c] = excelValueConverter.ToExcel(a[c], disposer);
                }
                return a;
            }

            return excelValueConverter.ToExcel(value, disposer);
        }
    }
}