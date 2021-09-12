using System.Collections.Generic;
using System.Linq;

namespace ExLibris.Core
{
    public class ExcelMatrixAccessor
    {
        private readonly ExcelValueConverter evConverter;
        private readonly object[,] excelMatrix;

        public readonly int RowSize;
        public readonly int ColumnSize;

        public ExcelMatrixAccessor(ExcelValueConverter evConverter, object[,] excelMatrix)
        {
            this.evConverter = evConverter;
            this.excelMatrix = excelMatrix;

            this.RowSize = excelMatrix.GetLength(0);
            this.ColumnSize = excelMatrix.GetLength(1);
        }

        public object this[int row, int column] => evConverter.ToValue(excelMatrix[row, column]);

        public IRowAccessor Row(int row) => new ExcelRowAccessor(this, row);

        public IEnumerable<IRowAccessor> Rows => Enumerable.Range(0, RowSize).Select(row => Row(row));

        public interface IRowAccessor
        {
            int ColumnSize { get; }

            object this[int column] { get; }

            IEnumerable<object> Values {get;}
        }

        class ExcelRowAccessor : IRowAccessor
        {
            public int ColumnSize => matrixAccessor.ColumnSize;

            private ExcelMatrixAccessor matrixAccessor;
            private int row;

            public ExcelRowAccessor(ExcelMatrixAccessor matrixAccessor, int row)
            {
                this.matrixAccessor = matrixAccessor;
                this.row = row;
            }

            public object this[int column] => matrixAccessor[row, column];

            public IEnumerable<object> Values => Enumerable.Range(0, ColumnSize).Select(column => this[column]);
        }
    }
}
