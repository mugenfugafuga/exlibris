using System.Collections.Generic;

namespace Exlibris.Excel
{
    public interface IExcelRow
    {
        int GetSize();

        IEnumerable<IExcelValue> Values { get; }
        IExcelValue this[int column] { get; }
        IExcelValue First { get; }
    }
}