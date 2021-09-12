using System.Collections.Generic;
using System.Linq;

namespace ExLibris.Core
{
    public class ExcelMatrixBuilder
    {
        private readonly ExcelValueConverter evConverter;
        private readonly List<List<object>> matrix;

        public ExcelMatrixBuilder(ExcelValueConverter evConverter, int rowSize, int columnSize)
        {
            this.evConverter = evConverter;
            this.matrix = new List<List<object>>();

            ResizeColumn(rowSize - 1, columnSize - 1);
        }

        public ExcelMatrixBuilder AddValue(int row, int column, object value)
        {
            ResizeColumn(row, column);
            matrix[row][column] = evConverter.ToExcel(value);
            return this;
        }

        public object this[int row, int column]
        {
            get => matrix[row][column];
            set
            {
                AddValue(row, column, value);
            }
        }

        public IExcelRowBuilder Row(int row)
        {
            ResizeRow(row);
            return new RowBuilder(this, row);
        }

        public IExcelRowBuilder NewRow()
        {
            var row = RowCount;
            ResizeRow(row);
            return new RowBuilder(this, row);
        }


        public int RowCount => matrix.Count;

        public int ColumnCount => matrix.Max(r => r.Count);

        public object[,] BuildExcelMatrix()
        {
            var excel = new object[RowCount, ColumnCount];

            for(var r =0; r < RowCount; ++r)
            {
                var row = matrix[r];
                for(var c = 0; c < row.Count; ++c)
                {
                    excel[r, c] = row[c];
                }
            }

            return excel;
        }

        private void ResizeRow(int row)
        {
            for(var r = matrix.Count; r <= row; ++r)
            {
                matrix.Add(new List<object>());
            }
        }

        private void ResizeColumn(int row, int column)
        {
            ResizeRow(row);
            var r = matrix[row];

            for(var c = r.Count; c <= column; ++c)
            {
                r.Add(null);
            }
        }

        public interface IExcelRowBuilder
        {
            object this[int column] { get;set; }
        }

        private class RowBuilder : IExcelRowBuilder
        {
            private ExcelMatrixBuilder matrixBuilder;
            private int row;

            public RowBuilder(ExcelMatrixBuilder matrixBuilder, int row)
            {
                this.matrixBuilder = matrixBuilder;
                this.row = row;
            }

            public object this[int column]
            {
                get => matrixBuilder[row, column];
                set => matrixBuilder[row, column] = value;
            }
        }
    }
}
