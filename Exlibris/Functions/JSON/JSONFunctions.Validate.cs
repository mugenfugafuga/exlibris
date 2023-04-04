using ExcelDna.Integration;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;

namespace Exlibris.Functions.JSON;

partial class JSONFunctions
{
    [ExcelFunction(
        Category = Category,
        Name = $"{Category}.{nameof(Validate)}",
        Description = "validate the JSON")]
    public static object Validate(
        [ExcelArgument(AllowReference = true, Description = "JSON management object")] object json,
        [ExcelArgument(AllowReference = true, Description = "JSON Schema management object")] object schema,
        [ExcelArgument(Description = "optional argument.if the argument is invalid, the method will not execute the process.")] object identifier
)
        => ExlibrisAddin.GetFunctionSupport().NoError(identifier).ErrorValueIfThrown(support =>
        {
            var jsonVal = support.NotDateTime(json, nameof(json)).ShouldBeScalar().GetValueOrThrow<JToken>();
            var schemaVal = support.NotDateTime(schema, nameof(schema)).ShouldBeScalar().GetValueOrThrow<JSchema>();

            var serializer = support.JSONSerializer;
            jsonVal.Validate(schemaVal);
            return true;
        });
}

