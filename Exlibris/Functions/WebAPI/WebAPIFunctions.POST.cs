using ExcelDna.Integration;
using Exlibris.Core.WebAPI;

namespace Exlibris.Functions.WebAPI;
partial class WebAPIFunctions
{
    [ExcelFunction(
    Category = Category,
    Name = $"{Category}.{nameof(POST)}",
    Description = "POST")]
    public static object POST(
        [ExcelArgument(AllowReference = true, Description = "url")] object url,
        [ExcelArgument(AllowReference = true, Description = "JSON management object, string value, or key-value pairs for Form data.")] object data,
        [ExcelArgument(AllowReference = true, Description = "optional argument. Content-Type. default: json object -> application/json, string -> text/plain, key-value pairs for Form data -> application/x-www-form-urlencoded")] object contentType,
        [ExcelArgument(AllowReference = true, Description = "optional argument. client, authentication or headers")] object parameter,
        [ExcelArgument(Description = "optional argument.if the argument is invalid, the method will not execute the process.")] object identifier)
        => ExlibrisAddin.GetFunctionSupport().ErrorValueIfThrown(support =>
        {
            var requestUri = support.NotDateTime(url, nameof(url)).ShouldBeScalar().GetValueOrThrow<string>();
            var dataVal = support.NotDateTime(data, nameof(data));
            var contentTypeVal = support.NotDateTime(contentType, nameof(contentType)).ShouldBeScalar();
            var paramVal = support.NotDateTime(parameter, nameof(parameter));

            return support.Observe((observer, disposer, cancellationToken) => Task.Run(async () =>
            {
                using var client = paramVal.IfScalar()?.TryGetValue<HttpClient>() ?? WebAPIUtil.NewClinet();

                var request = CreateRequest(HttpMethod.Post, requestUri, paramVal, CreateHttpContent(support, dataVal, contentTypeVal), support);

                var response = support.CacheDisposable(await client.SendAsync(request, cancellationToken), disposer);

                await PostProcessingAsync(support, observer, response);
            }, cancellationToken), url, data, parameter, contentType, identifier);
        });
}
