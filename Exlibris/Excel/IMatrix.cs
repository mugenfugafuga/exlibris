using System.Collections.Generic;

namespace Exlibris.Excel
{
    public interface IMatrix
    {
        ExcelAddress? Address { get; }

        int ColumnSize { get; }

        int RowSize { get; }

        IEnumerable<IExcelRow> Rows { get; }

        IEnumerable<IExcelValue> Values { get; }
    }
}