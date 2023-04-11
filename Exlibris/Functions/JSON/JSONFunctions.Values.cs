using ExcelDna.Integration;
using Exlibris.Core.JSONs.Extension;
using Newtonsoft.Json.Linq;

namespace Exlibris.Functions.JSON;
partial class JSONFunctions
{
    [ExcelFunction(
        Category = Category,
        Name = $"{Category}.{nameof(Values)}",
        Description = "display JSONPath and Value Pairs."
    )]
    public static object Values(
        [ExcelArgument(AllowReference = true, Description = "JSON management object")] object json,
        [ExcelArgument(AllowReference = true, Description = "optional argument. specifies the JSON Path of the element to display. If specified, only the element corresponding to the JSON Path will be displayed. if not specified, displays all JSON Path and Value Pairs.")] object jsonPath,
        [ExcelArgument(AllowReference = true, Description = "optional argument. display paths")] object withPath,
        [ExcelArgument(Description = "optional argument.if the argument is invalid, the method will not execute the process.")] object identifier)
        => ExlibrisAddin.GetFunctionSupport().NoError(identifier).ErrorValueIfThrown(support =>
        {
            var jo = support.NotDateTime(json, nameof(json)).ShouldBeScalar().GetValueOrThrow<JToken>();
            var jsonPathVal = support.NotDateTime(jsonPath, nameof(jsonPath)).ShouldBeScalar();
            var withPathVal = support.NotDateTime(withPath, nameof(withPath)).ShouldBeScalar();

            return support.Observe(false, (_0, _1) =>
            {
                var serializer = support.JSONSerializer;

                var values = jsonPathVal.TryGetValue<string>(out var jpath) ?
                    serializer.FilterJsons(jo, jpath) :
                    serializer.GetValues(jo).Select(v => (v.Path, v.Value.GetBase()));

                if (withPathVal.Value == null)
                {
                    var builder = support.NewMatrixBuilder();

                    foreach (var (path, value) in values)
                    {
                        builder.NewRow().Add(path).Add(value.GetValueOrThis()).Close();
                    }

                    var result = builder.Matrix() as object[,] ?? throw new ArgumentException(nameof(values));

                    if (result.GetLongLength(0) == 1)
                    {
                        return result[0, 1];
                    }
                    else
                    {
                        return result;
                    }
                }

                if (withPathVal.GetValueOrThrow<bool>())
                {
                    var builder = support.NewMatrixBuilder();

                    foreach (var (path, value) in values)
                    {
                        builder.NewRow().Add(path).Add(value.GetValueOrThis()).Close();
                    }

                    return builder.Matrix() as object[,] ?? throw new ArgumentException(nameof(values));
                }
                else
                {
                    var builder = support.NewMatrixBuilder();

                    foreach (var (_, value) in values)
                    {
                        builder.NewRow().Add(value.GetValueOrThis()).Close();
                    }

                    return builder.Matrix() as object[,] ?? throw new ArgumentException(nameof(values));
                }
            }, json, jsonPath, withPath, identifier);
        });
}
