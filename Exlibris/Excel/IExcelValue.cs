namespace Exlibris.Excel;

public interface IExcelValue : IScalar, IMatrix
{
    ExcelDataType DataType { get; }

    IScalar ShouldBeScalar();

    IMatrix ShouldBeMatrix();

    IScalar? IfScalar();

    IMatrix? IfMatrix();
}
