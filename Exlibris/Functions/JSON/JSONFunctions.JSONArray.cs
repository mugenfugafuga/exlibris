using ExcelDna.Integration;
using Exlibris.Excel;
using System.Linq;

namespace Exlibris.Functions.JSON
{
    partial class JSONFunctions
    {
        [ExcelFunction(
            Category = Category,
            Name = Category + ".JSONArray",
            Description = "generates a JSON array management object in memory from table-structured data.")]
        public static object JSONArray(
            [ExcelArgument(AllowReference = true, Description = "table-structured data. Uses the field name in the first row as the JSONPath and each subsequent row as the value of the corresponding array element.")] object table,
            [ExcelArgument(AllowReference = true, Description = "optional argument. select the DateTimeDetector")] object dateTimeDetection,
            [ExcelArgument(Description = "optional argument.if the argument is invalid, the method will not execute the process.")] object identifier)
            => ExlibrisAddin.GetFunctionSupport().NoError(identifier).ErrorValueIfThrown(support =>
            {
                var dtd = support.NotDateTime(dateTimeDetection, nameof(dateTimeDetection)).TryGetValue<DateTimeDetector>(out var d) ? d : null;
                var matrix = support.ShouldBeMatrix(table, nameof(table), dtd);

                return support.ObserveObjectRegistration(
                    () =>
                    {
                        var serializer = support.JSONSerializer;

                        if (matrix.RowSize == 2)
                        {
                            var builder = serializer.NewJSONBuilder();

                            var paths = matrix.Rows.First();
                            var values = matrix.Rows.Skip(1).First();

                            foreach (var (path, value) in paths.Values.Zip(values.Values, (p, v) => (p, v)))
                            {
                                builder.Append(path.GetValueOrThrow<string>(), value.Value);
                            }

                            return builder.Build();
                        }
                        else
                        {
                            var paths = matrix.Rows.First().Values.Select(p => p.GetValueOrThrow<string>());

                            var arrayBuilder = serializer.NewJSONBuilder();

                            foreach (var row in matrix.Rows.Skip(1))
                            {
                                var builder = serializer.NewJSONBuilder();

                                foreach (var (path, value) in paths.Zip(row.Values, (p, v) => (p, v)))
                                {
                                    builder.Append(path, value.Value);
                                }

                                arrayBuilder.AddArrayElement(builder.Build());
                            }

                            return arrayBuilder.Build();
                        }
                    }, table, dateTimeDetection, identifier);
            });
    }
}