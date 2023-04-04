namespace Exlibris.Excel;
public interface IExcelRow
{
    public int Size { get; }
    IEnumerable<IExcelValue> Values { get; }
    IExcelValue this[int column] { get; }
    IExcelValue First { get; }
}
