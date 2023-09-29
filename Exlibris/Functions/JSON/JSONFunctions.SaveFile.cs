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
            Name = Category + ".SaveFile",
            Description = "saves the JSON string to a file using the specified JSON management object.")]
        public static object SaveFile(
            [ExcelArgument(AllowReference = true, Description = "JSON management object or JSON Schema management object.")] object json,
            [ExcelArgument(AllowReference = true, Description = "the path of the file where the JSON string will be saved.")] object filePath,
            [ExcelArgument(AllowReference = true, Description = "optional argument. determines whether to indent the generated JSON or not. The default value is True, which means that the JSON will be indented by default.")] object pretty,
            [ExcelArgument(Description = "optional argument.if the argument is invalid, the method will not execute the process.")] object identifier)
            => ExlibrisAddin.GetFunctionSupport().NoError(identifier).ErrorValueIfThrown(support =>
            {
                var jsonVal = support.NotDateTime(json, nameof(json)).ShouldBeScalar();
                var file = support.NotDateTime(filePath, nameof(filePath)).ShouldBeScalar().GetValueOrThrow<string>();
                var prtty = support.NotDateTime(pretty, nameof(pretty)).ShouldBeScalar().GetValueOrDefault(true);

                var serilalizer = support.JSONSerializer;

                if (jsonVal.TryGetValue<JToken>(out var jsn))
                {
                    serilalizer.SaveFile(jsn, file, prtty);
                    return file;
                }

                if (jsonVal.TryGetValue<JSchema>(out var jsonSchema))
                {
                    serilalizer.SaveSchemaFile(jsonSchema, file, prtty);
                    return file;
                }

                throw new NotImplementedException($"unsupported type. type : {jsonVal.Value?.GetType()}");
            });
    }
}