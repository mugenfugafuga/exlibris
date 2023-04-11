using ExcelDna.Integration;
using Exlibris.Excel;
using System.Collections.Specialized;
using System.Web;

namespace Exlibris.Functions.WebAPI;

partial class WebAPIFunctions
{
    [ExcelFunction(
    Category = Category,
    Name = $"{Category}.{nameof(URL)}",
    Description = "build URL.")]
    public static object URL(
        [ExcelArgument(AllowReference = true, Description = "base URL or parameters")] object value,
        [ExcelArgument(AllowReference = true, Description = "optional argument. parameters")] object parameters,
        [ExcelArgument(AllowReference = true, Description = "optional argument. setting the argument to ‘true’as an object in the memory, while setting it to ‘false’ displays the text on Excel. default : false")] object asObject,
        [ExcelArgument(Description = "optional argument.if the argument is invalid, the method will not execute the process.")] object identifier)
        => ExlibrisAddin.GetFunctionSupport().ErrorValueIfThrown(support =>
        {
            var val = support.NotDateTime(value, nameof(value));
            var paramsVal = support.NotDateTime(parameters, nameof(parameters));

            if (support.NotDateTime(asObject, nameof(asObject)).ShouldBeScalar().GetValueOrDefault(false))
            {
                return support.ObserveObjectRegistration(() =>
                {
                    return BuildUri(val, paramsVal).ToString();
                }, value, parameters, asObject, identifier);
            }
            else
            {
                return BuildUri(val, paramsVal).ToString();
            }
        });

    private static string BuildUri(IExcelValue value, IExcelValue parameters)
    {
        if (value.DataType == ExcelDataType.Scalar)
        {
            var builder = new UriBuilder(value.ShouldBeScalar().GetValueOrThrow<string>());

            if (!parameters.IsNull)
            {
                builder.Query = BuildQuery(parameters.ShouldBeMatrix()).ToString();
            }

            return builder.Uri.ToString();
        }
        else
        {
            return BuildQuery(
                value.ShouldBeMatrix(),
                parameters.IfMatrix()
                )?.ToString() ?? throw new Exception("unreachable");
        }
    }

    private static NameValueCollection BuildQuery(params IMatrix?[] matrixes)
    {
        var rows = matrixes.Where(m => m != null).SelectMany(m => m?.Rows ?? throw new Exception("unreachable")); 

        var query = HttpUtility.ParseQueryString(string.Empty);

        foreach(var row in rows)
        {
            query[row[0].GetValueOrThrow<string>()] = row[1].GetValueOrThrow<string>();
        }

        return query;
    }
}
