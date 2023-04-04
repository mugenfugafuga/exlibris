using ExcelDna.Integration;

namespace Exlibris.Functions.JSON;
partial class JSONFunctions
{
    [ExcelFunction(
        Category = Category,
        Name = $"{Category}.{nameof(Schema)}",
        Description = "generates a JSON Schema management object in memory.")]
    public static object Schema(
        [ExcelArgument(AllowReference = true, Description = "json string, file path or C# class name. if the specified C# class can be serialized in json, Generate a schema managed object for it.")] object source,
        [ExcelArgument(Description = "optional argument.if this argument is an error, don't perform the calculation.")] object identifier
)
        => ExlibrisAddin.GetFunctionSupport().NoError(identifier).ErrorValueIfThrown(support =>
        {
            var src = support.NotDateTime(source, nameof(source)).ShouldBeScalar().GetValueOrThrow<string>();

            return support.ObserveObjectRegistration(() =>
            {
                var serilalizer = support.JSONSerializer;

                if (File.Exists(src))
                {
                    return serilalizer.LoadSchemaFile(src);
                }

                try
                {
                    // try to generate the schema of C# class.
                    return serilalizer.GetSchema(src);
                }
                catch
                {
                    return serilalizer.DeserializeSchema(src);
                }
            }, src, identifier);
        });
}
