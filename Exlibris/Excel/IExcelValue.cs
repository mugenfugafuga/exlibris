namespace Exlibris.Excel;

public interface IExcelValue : IScalar, IMatrix
{
    ExcelDataType DataType { get; }

    IScalar ShouldBeScalar();

    IMatrix ShouldBeMatrix();
}
