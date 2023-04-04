namespace Exlibris.Excel;
public class MatrixBuilder
{
    private readonly List<List<object?>> rows = new();

    public int RowCount => rows.Count;

    public int ColumnCount => rows.Max(r => r.Count);

    public MatrixBuilder Add(object? value)
        => NewRow().Add(value).Close();

    public Row NewRow()
    {
        var vs = new List<object?>();
        rows.Add(vs);
        return new Row(this, vs);
    }

    public object? Build()
    {
        if (RowCount > 1 || ColumnCount > 1)
        {
            return Matrix();
        }
        else
        {
            return Single();
        }
    }

    public object? Single()
    {
        try
        {
            return rows[0][0];
        }
        catch(Exception)
        {
            return null;
        }
    }

    public object?[,] Matrix()
    {
        var rowSize = RowCount;
        var columnSize = ColumnCount;

        var ret = new object?[rowSize, columnSize];

        for (var r = 0; r < rowSize; ++r)
        {
            var rv = rows[r];
            for (var c = 0; c < rv.Count; ++c)
            {
                ret[r, c] = rv[c];
            }
        }

        return ret;
    }

    public readonly struct Row
    {
        private readonly MatrixBuilder builder;
        private readonly List<object?> values;

        internal Row(MatrixBuilder builder, List<object?> values)
        {
            this.builder = builder;
            this.values = values;
        }

        public Row Add(object? value)
        {
            values.Add(value);
            return this;
        }

        public MatrixBuilder Close() => builder;
    }

}
