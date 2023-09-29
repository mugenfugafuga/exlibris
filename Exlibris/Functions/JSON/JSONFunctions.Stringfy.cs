using ExcelDna.Integration;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System;

namespace Exlibris.Functions.JSON
{
    partial class JSONFunctions
    {
        [ExcelFunction(
            Category = Category,
            Name = Category + ".Stringfy",
            Description = "generates a JSON string from the specified JSON management object.")]
        public static object Stringfy(
            [ExcelArgument(AllowReference = true, Description = "JSON management object or JSON Schema management object.")] object json,
            [ExcelArgument(Description = "optional argument.if the argument is invalid, the method will not execute the process.")] object identifier)
            => ExlibrisAddin.GetFunctionSupport().NoError(identifier).ErrorValueIfThrown(support =>
            {
                var jsonVal = support.NotDateTime(json, nameof(json)).ShouldBeScalar();
                var serilalizer = support.JSONSerializer;

                if (jsonVal.TryGetValue<JToken>(out var jsn))
                {
                    return serilalizer.Serialize(jsn);
                }

                if (jsonVal.TryGetValue<JSchema>(out var schema))
                {
                    return serilalizer.SerializeSchema(schema);
                }

                throw new NotImplementedException($"unsupported type. the type of {nameof(json)} : {jsonVal.Value?.GetType().ToString() ?? "null"}");
            });
    }
}