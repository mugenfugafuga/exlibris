using ExcelDna.Integration;
using Exlibris.Excel;
using System;

namespace Exlibris
{
    internal static class ExlibrisUtil
    {
        public static ExcelAddress? GetAddress(ExcelReference cell)
        {
            if (cell == null) { return null; }
            return new ExcelAddress(cell.RowFirst, cell.RowLast, cell.ColumnFirst, cell.ColumnLast, GetSheetName(cell));
        }

        private static string GetSheetName(ExcelReference cell)
        {
            try
            {
                return (string)XlCall.Excel(XlCall.xlSheetNm, cell);
            }
            catch
            {
                return $"[{cell.SheetId}]";
            }
        }

        public static bool TryGet<T>(Func<T> func, out T val)
        {
            try
            {
                val = func();
                return true;
            }
            catch
            {
                val = default;
                return false;
            }
        }

        public static RawExcel GetRawExcel(object excel, string name)
        {
            var reference = excel is ExcelReference er ? er : null;
            var excl = reference?.GetValue() ?? excel;
            var address = GetAddress(reference);

            return new RawExcel(address, excl, name);
        }
    }
}