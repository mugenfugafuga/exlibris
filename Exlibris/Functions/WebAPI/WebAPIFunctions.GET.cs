using ExcelDna.Integration;
using Exlibris.Core.WebAPI;

namespace Exlibris.Functions.WebAPI;
partial class WebAPIFunctions
{
    [ExcelFunction(
    Category = Category,
    Name = $"{Category}.{nameof(GET)}",
    Description = "GET")]
    public static object GET(
        [ExcelArgument(AllowReference = true, Description = "url")] object url,
        [ExcelArgument(AllowReference = true, Description = "optional argument. client, authentication or headers")] object parameter,
        [ExcelArgument(Description = "optional argument.if the argument is invalid, the method will not execute the process.")] object identifier)
        => ExlibrisAddin.GetFunctionSupport().ErrorValueIfThrown(support =>
        {
            var requestUri = support.NotDateTime(url, nameof(url)).ShouldBeScalar().GetValueOrThrow<string>();
            var paramVal = support.NotDateTime(parameter, nameof(parameter));

            return support.Observe((observer, disposer, cancellationToken) => Task.Run(async () =>
            {
                using var client = paramVal.IfScalar()?.TryGetValue<HttpClient>() ?? WebAPIUtil.NewClinet();

                var request = CreateRequest(HttpMethod.Get, requestUri, paramVal, support);

                var response = support.CacheDisposable(await client.SendAsync(request, cancellationToken), disposer);

                await PostProcessingAsync(support, observer, response);

            }, cancellationToken), url, parameter, identifier);
        });
}
