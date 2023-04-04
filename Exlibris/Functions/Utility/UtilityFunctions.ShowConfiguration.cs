using ExcelDna.Integration;
using Exlibris.Core.JSONs.Extension;

namespace Exlibris.Functions.Utility;
partial class UtilityFunctions
{
    [ExcelFunction(
        Category = Category,
        Name = $"{Category}.{nameof(ShowConfiguration)}",
        Description = "output Exlibris configuration",
        IsHidden = true)]
    public static object? ShowConfiguration(
        [ExcelArgument(Description = "optional argument.if the argument is invalid, the method will not execute the process.")] object identifier
        )
        => ExlibrisAddin.GetFunctionSupport().NoError(identifier).ErrorValueIfThrown(support =>
        {
            var configuration = support.ExlibrisConfiguration;
            var parser = support.JSONSerializer;

            var jsonObject = parser.FromObject(configuration);

            var builder = support.NewMatrixBuilder();

            foreach (var (path, value) in parser.GetValues(jsonObject))
            {
                builder.NewRow().Add(path).Add(value.GetValueOrThis()).Close();
            }

            return builder.Build();
        });
}
