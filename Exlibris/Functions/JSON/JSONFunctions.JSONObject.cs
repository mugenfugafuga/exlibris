using ExcelDna.Integration;
using Exlibris.Core.Reflection;
using Exlibris.Excel;
using Newtonsoft.Json.Linq;

namespace Exlibris.Functions.JSON;

partial class JSONFunctions
{
    [ExcelFunction(
        Category = Category,
        Name = $"{Category}.{nameof(JSONObject)}",
        Description = "generates a JSON management object in memory.")]
    public static object JSONObject(
        [ExcelArgument(AllowReference = true, Description = "json string, file path or JSONPath and Value Pairs.")] object source,
        [ExcelArgument(AllowReference = true, Description = "optional argument. select the DateTimeDetector")] object dateTimeDetection,
        [ExcelArgument(AllowReference = true, Description = "optional argument. C# class name. convert to the specified class.")] object className,
        [ExcelArgument(Description = "optional argument.if the argument is invalid, the method will not execute the process.")] object identifier
        )
        => ExlibrisAddin.GetFunctionSupport().NoError(identifier).ErrorValueIfThrown(support =>
        {
            var dtd = support.NotDateTime(dateTimeDetection, nameof(dateTimeDetection)).TryGetValue<DateTimeDetector>(out var d) ? d : null;
            var src = support.NewExcelValue(source, nameof(source), dtd);
            var classNameVal = support.NotDateTime(className, nameof(className)).ShouldBeScalar();

            return support.ObserveObjectRegistration(
                () =>
                {
                    var json = CreateJSONObject(support, src);

                    if (classNameVal.TryGetValue<string>(out var cn))
                    {
                        var type = ReflectionUtil.GetType(cn);
                        return support.JSONSerializer.ToObject(json, type) ?? throw new ArgumentException($"can not convert to {type}");
                    }
                    else
                    {
                        return json;
                    }
                }, source, className, identifier);
        });

    private static JToken CreateJSONObject(ExlibrisExcelFunctionSupport support, IExcelValue value)
    {
        switch(value.DataType)
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

    private static JToken CreateJSONObjectFromMatrix(ExlibrisExcelFunctionSupport support, IMatrix matrix)
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

    private static JToken CreateJSONObjectFromString(ExlibrisExcelFunctionSupport support, string value)
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