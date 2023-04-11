using ExcelDna.Integration;
using Exlibris.Core.WebAPI;

namespace Exlibris.Functions.WebAPI;
partial class WebAPIFunctions
{
    [ExcelFunction(
        Category = Category,
        Name = $"{Category}.{nameof(Examples)}",
        Description = "setting examples")]
    public static object Examples(
        [ExcelArgument(Description = "optional argument.if this argument is an error, don't perform the calculation.")] object identifier)
        => ExlibrisAddin.GetFunctionSupport().NoError(identifier).ErrorValueIfThrown(support =>
        {
            return support.Observe(false, (observser, diposer) =>
            {
                var serializer = support.JSONSerializer;

                var builer = support.NewMatrixBuilder();

                builer
                .NewRow().Add("basic authentication").Add(serializer.FromObject(BasicAuthExample)).Close()
                .NewRow().Add("http client setting schema").Add(serializer.GetSchema("Exlibris.Core.WebAPI.HttpClientSetting")).Close()
                .NewRow().Add("http request setting schema").Add(serializer.GetSchema("Exlibris.Core.WebAPI.HttpRequestSetting")).Close();

                return builer.Build();

            }, identifier);
        });

    private static readonly HttpRequestSetting BasicAuthExample = new HttpRequestSetting
    {
        Headers = new HttpHeaders
        {
            AuthenticationHeader = new HttpAuthenticationHeader
            {
                Basic = new BasicAuthentication
                {
                    User = "user_account",
                    Password = "password",
                }
            }
        }
    };
}
