using Exlibris.Excel;
using System.Diagnostics.CodeAnalysis;

namespace Exlibris;

partial class ExlibrisExcelFunctionSupport
{
    private readonly struct ScalarExcelValue : IExcelValue
    {
        public ExcelDataType DataType => ExcelDataType.Scalar;

        public IScalar ShouldBeScalar() => this;

        public IMatrix ShouldBeMatrix()
        {
            throw name == null ?
                new InvalidOperationException("data type is not matrix.") :
                new InvalidOperationException($"{name} is not matrix.");
        }

        public ExcelAddress? Address { get; }

        public object? Value { get; }

        public int ColumnSize => 1;

        public int RowSize => 1;

        public IEnumerable<IExcelRow> Rows
        {
            get
            {
                yield return new Row(this);
            }
        }

        public IEnumerable<IExcelValue> Values
        {
            get
            {
                yield return this;
            }
        }

        private readonly string? name;

        public ScalarExcelValue(ExcelAddress? address, object excel, ExcelSingleValueConverter converter, string? name, DateTimeDetector? dateTimeDetector)
        {
            Address = address;
            this.name = name;
            Value = converter.FromExcel(excel, Address, dateTimeDetector);
        }

        public bool TryGetValue<T>([MaybeNullWhen(false)] out T value)
        {
            var v = GetValueInternal<T>(Value);

            if (v != null)
            {
                value = (T)v;
                return true;
            }

            value = default;
            return false;
        }

        public T GetValueOrThrow<T>()
        {
            var v = GetValueInternal<T>(Value);

            if (v != null) { return (T)v; }

            throw name == null ?
                new ArgumentException($"cannot convert to {typeof(T)}, cell : {Address?.Address}") :
                new ArgumentException($"{name} cannot convert to {typeof(T)}, cell : {Address?.Address}");
        }

        public T GetValueOrDefault<T>(T defaultValue)
        {
            if (Value == null)
            {
                return defaultValue;
            }

            return GetValueOrThrow<T>();
        }

        private static object? GetValueInternal<T>(object? arg)
        {
            if (arg == null) { return null; }

            if (typeof(T).Equals(typeof(int))) { return Convert.ToInt32(arg); }
            if (typeof(T).Equals(typeof(long))) { return Convert.ToInt64(arg); }

            if (typeof(T).IsEnum) { return Enum.ToObject(typeof(T), Convert.ToInt32(arg)); }

            if (arg is T val) { return val; }

            return null;
        }

        private readonly struct Row : IExcelRow
        {
            public IExcelValue this[int column]
            {
                get
                {
                    if (column == 0)
                    {
                        return value;
                    }

                    throw new IndexOutOfRangeException();
                }
            }

            public int Size => 1;

            public IEnumerable<IExcelValue> Values
            {
                get
                {
                    yield return value;
                }
            }


            public IExcelValue First => value;

            private readonly ScalarExcelValue value;

            public Row(ScalarExcelValue value)
            {
                this.value = value;
            }
        }
    }
}
