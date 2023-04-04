using Exlibris.Excel;
using Newtonsoft.Json.Linq;

namespace Exlibris.Functions.JSON;
static class JSONFuncUtil
{
    public static JToken CreateJSONObject(ExlibrisExcelFunctionSupport support, IExcelValue value)
    {
        switch (value.DataType)
        {
            case ExcelDataType.Matrix:
                return CreateJSONObjectFromMatrix(support, value.ShouldBeMatrix());

            case ExcelDataType.Scalar:
                {
                    if (value.TryGetValue<string>(out var vs))
                    {
                        return CreateJSONObjectFromString(support, vs);
                    }
                    else
                    {
                        // try to generate JSON management object from C# object;
                        return support.JSONSerializer.FromObject(value.Value);
                    }

                }

            default: throw new NotImplementedException(value.DataType.ToString());
        }
    }

    public static JToken CreateJSONObjectFromMatrix(ExlibrisExcelFunctionSupport support, IMatrix matrix)
    {
        var builder = support.JSONSerializer.NewJSONBuilder();

        foreach (var row in matrix.Rows)
        {
            var path = row[0].GetValueOrThrow<string>();
            var val = row[1].Value;

            builder.Append(path, val);
        }

        return builder.Build();
    }

    public static JToken CreateJSONObjectFromString(ExlibrisExcelFunctionSupport support, string value)
    {
        if (File.Exists(value))
        {
            return support.JSONSerializer.LoadFile(value);
        }

        if (ExlibrisUtil.TryGet(() => support.JSONSerializer.Deserialize(value), out var jt))
        {
            return jt;
        }

        return support.JSONSerializer.FromObject(value);
    }
}
