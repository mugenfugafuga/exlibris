using System.Collections.Generic;
using System;

namespace Exlibris.Excel
{
    public readonly struct ExcelAddress : IEquatable<ExcelAddress>
    {
        private static string Column(int columnNum) => ToColumnName(columnNum + 1);
        private static string ToColumnName(int source)
            => source < 1 ? string.Empty : ToColumnName((source - 1) / 26) + (char)(65 + ((source - 1) % 26));

        public ExcelAddress(int rowFirst, int rowLast, int columnFirst, int columnLast, string sheetName)
        {
            RowFirst = rowFirst;
            RowLast = rowLast;
            ColumnFirst = columnFirst;
            ColumnLast = columnLast;
            SheetName = sheetName;
        }

        public int RowFirst { get; }
        public int RowLast { get; }
        public int ColumnFirst { get; }
        public int ColumnLast { get; }
        public string SheetName { get; }
        public string Address => $"{SheetName}!{Column(ColumnFirst)}{RowFirst + 1}:{Column(ColumnLast)}{RowLast + 1}";

        public ExcelAddress Relative(int row, int column)
            => new ExcelAddress(RowFirst + row, RowFirst + row, ColumnFirst + column, ColumnFirst + column, SheetName);

        public ExcelAddress RowAddress(int row)
            => new ExcelAddress(RowFirst + row, RowFirst + row, ColumnFirst, ColumnLast, SheetName);

        public ExcelAddress ColumnAddress(int column)
            => new ExcelAddress(RowFirst, RowLast, ColumnFirst + column, ColumnFirst + column, SheetName);

        public IEnumerable<ExcelAddress> CellAddresses()
        {
            for (var row = RowFirst; row <= RowLast; ++row)
            {
                for (var column = ColumnFirst; column <= ColumnLast; ++column)
                {
                    yield return Relative(row, column);
                }
            }
        }

        public bool Contains(ExcelAddress? address)
        {
            if (address == null)
            {
                return false;
            }

            return SheetName == address?.SheetName &&
                RowFirst <= address?.RowFirst &&
                RowLast >= address?.RowLast &&
                ColumnFirst <= address?.ColumnFirst &&
                ColumnLast >= address?.ColumnLast;
        }

        public override string ToString() => Address;

        public override bool Equals(object obj)
        {
            return obj is ExcelAddress address && Equals(address);
        }

        public bool Equals(ExcelAddress other)
        {
            return RowFirst == other.RowFirst &&
                   RowLast == other.RowLast &&
                   ColumnFirst == other.ColumnFirst &&
                   ColumnLast == other.ColumnLast &&
                   SheetName == other.SheetName;
        }

        public override int GetHashCode()
        {
            int hashCode = 1494207578;
            hashCode = hashCode * -1521134295 + RowFirst.GetHashCode();
            hashCode = hashCode * -1521134295 + RowLast.GetHashCode();
            hashCode = hashCode * -1521134295 + ColumnFirst.GetHashCode();
            hashCode = hashCode * -1521134295 + ColumnLast.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(SheetName);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Address);
            return hashCode;
        }

        public static bool operator ==(ExcelAddress left, ExcelAddress right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ExcelAddress left, ExcelAddress right)
        {
            return !(left == right);
        }
    }
}