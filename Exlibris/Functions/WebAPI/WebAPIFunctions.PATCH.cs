using ExcelDna.Integration;
using Exlibris.Core.WebAPI;

namespace Exlibris.Functions.WebAPI;
partial class WebAPIFunctions
{
    [ExcelFunction(
    Category = Category,
    Name = $"{Category}.{nameof(PATCH)}",
    Description = "PATCH")]
    public static object PATCH(
        [ExcelArgument(AllowReference = true, Description = "url")] object url,
        [ExcelArgument(AllowReference = true, Description = "JSON management object or string.")] object json,
        [ExcelArgument(AllowReference = true, Description = "optional argument. Content-Type. default: json object -> application/json, string -> text/plain")] object contentType,
        [ExcelArgument(AllowReference = true, Description = "optional argument. client, authentication or headers")] object parameter,
        [ExcelArgument(Description = "optional argument.if the argument is invalid, the method will not execute the process.")] object identifier)
        => ExlibrisAddin.GetFunctionSupport().ErrorValueIfThrown(support =>
        {
            var requestUri = support.NotDateTime(url, nameof(url)).ShouldBeScalar().GetValueOrThrow<string>();
            var jsonVal = support.NotDateTime(json, nameof(json)).ShouldBeScalar();
            var contentTypeVal = support.NotDateTime(contentType, nameof(contentType)).ShouldBeScalar();
            var paramVal = support.NotDateTime(parameter, nameof(parameter));

            return support.Observe((observer, disposer, cancellationToken) => Task.Run(async () =>
            {
                using var client = paramVal.IfScalar()?.TryGetValue<HttpClient>() ?? WebAPIUtil.NewClinet();

                var request = CreateRequest(HttpMethod.Patch, requestUri, paramVal, CreateHttpContent(support, jsonVal, contentTypeVal), support);

                var response = support.CacheDisposable(await client.SendAsync(request, cancellationToken), disposer);

                await PostProcessingAsync(support, observer, response);
            }, cancellationToken), url, json, parameter, contentType, identifier);
        });
}
