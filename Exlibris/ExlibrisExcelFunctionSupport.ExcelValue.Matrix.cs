using Exlibris.Excel;
using System.Diagnostics.CodeAnalysis;

namespace Exlibris;

partial class ExlibrisExcelFunctionSupport
{
    private class MatrixExcelValue : IExcelValue
    {
        private readonly object[,] matrix;
        private readonly ExcelSingleValueConverter converter;
        private readonly string? name;
        private readonly DateTimeDetector? dateTimeDetector;

        public MatrixExcelValue(ExcelAddress? address, object[,] matrix, ExcelSingleValueConverter converter, string? name, DateTimeDetector? dateTimeDetector)
        {
            this.matrix = matrix;
            RowSize = matrix.GetLength(0);
            ColumnSize = matrix.GetLength(1);
            this.converter = converter;
            this.Address = address;
            this.name = name;
            this.dateTimeDetector = dateTimeDetector;
        }

        public ExcelDataType DataType => ExcelDataType.Matrix;

        public IScalar ShouldBeScalar()
        {
            throw name == null ?
                new InvalidOperationException("data type is not scalar.") :
                new InvalidOperationException($"{name} is not scalar.");
        }

        public IMatrix ShouldBeMatrix() => this;

        public ExcelAddress? Address { get; }

        public object? Value
        {
            get
            {
                throw name == null ?
                    new InvalidOperationException("data type is not scalar.") :
                    new InvalidOperationException($"{name} is not scalar.");
            }
        }

        public bool TryGetValue<T>([MaybeNullWhen(false)] out T _)
        {
            throw name == null ?
                new InvalidOperationException("data type is not scalar.") :
                new InvalidOperationException($"{name} is not scalar.");
        }

        public T GetValueOrThrow<T>()
        {
            throw name == null ?
                new InvalidOperationException("data type is not scalar.") :
                new InvalidOperationException($"{name} is not scalar.");
        }

        public T GetValueOrDefault<T>(T defaultValue)
        {
            throw name == null ?
                new InvalidOperationException("data type is not scalar.") :
                new InvalidOperationException($"{name} is not scalar.");
        }

        public int ColumnSize { get; }

        public int RowSize { get; }

        public IEnumerable<IExcelRow> Rows
        {
            get
            {
                for (var row = 0; row < RowSize; ++row)
                {
                    yield return new ExcelRow(row, ColumnSize, Address?.RowAddress(row), matrix, converter, name, dateTimeDetector);
                }
            }
        }

        public IEnumerable<IExcelValue> Values
        {
            get
            {
                for (var row = 0; row < RowSize; ++row)
                {
                    for (var column = 0; column < ColumnSize; ++column)
                    {
                        yield return new ScalarExcelValue(Address?.Relative(row, column), matrix[row, column], converter, name, dateTimeDetector);
                    }
                }
            }
        }

        readonly struct ExcelRow : IExcelRow
        {
            public IExcelValue this[int column] => new ScalarExcelValue(address?.ColumnAddress(column), excelMatrix[row, column], converter, name, dateTimeDetector);

            public int Size => columnSize;

            public IEnumerable<IExcelValue> Values
            {
                get
                {
                    for (var column = 0; column < columnSize; ++column)
                    {
                        yield return new ScalarExcelValue(address?.ColumnAddress(column), excelMatrix[row, column], converter, name, dateTimeDetector);
                    }
                }
            }

            public IExcelValue First => new ScalarExcelValue(address?.ColumnAddress(0), excelMatrix[row, 0], converter, name, dateTimeDetector);

            private readonly int row;
            private readonly int columnSize;
            private readonly ExcelAddress? address;
            private readonly object[,] excelMatrix;
            private readonly ExcelSingleValueConverter converter;
            private readonly string? name;
            private readonly DateTimeDetector? dateTimeDetector;

            public ExcelRow(int row, int columnSize, ExcelAddress? address, object[,] excelMatrix, ExcelSingleValueConverter converter, string? name, DateTimeDetector? dateTimeDetector)
            {
                this.row = row;
                this.columnSize = columnSize;
                this.address = address;
                this.excelMatrix = excelMatrix;
                this.converter = converter;
                this.name = name;
                this.dateTimeDetector = dateTimeDetector;
            }
        }
    }
}
