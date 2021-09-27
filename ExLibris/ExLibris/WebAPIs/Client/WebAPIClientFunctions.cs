using ExcelDna.Integration;
using ExLibris.Core;
using ExLibris.Core.WebAPIs;
using System;

namespace ExLibris.WebAPIs.Client
{
    public static class WebAPIClientFunctions
    {
        static WebAPIClientFunctions()
        {
            ExLibrisContext.SecurityProtocolUpdateFunction.Invoke();
        }

        private const string categoryName = "ExLibris.WebAPIs.Client";
        private const string prefixFunctionName = categoryName + ".";

        [ExcelFunction(
            Name = prefixFunctionName + nameof(CreateWebAPIClientAsync),
            Category = categoryName)]
        public static object CreateWebAPIClientAsync(string baseUri, string headersHandle, object identifier)
        {
            var context = ExLibrisContext.DefaultContext;
            var support = context.GetFunctionCallSupport();

            return ExLibrisUtility.ExcelObserveObjectRegistrationAsync(
                nameof(CreateWebAPIClientAsync),
                support.ObjectRepository,
                () =>
                {
                    if (ExLibrisUtility.IsExcelError(identifier))
                    {
                        throw new ArgumentException($"{nameof(identifier)} is Error");
                    }

                    var bu = support.ToValueAsString(baseUri);

                    var buri = string.IsNullOrEmpty(bu) ?
                        null :
                        bu;
                    var headers = string.IsNullOrEmpty(headersHandle) ?
                        null :
                        support.ObjectRepository.GetObject<WebAPIHeaders>(headersHandle);

                    return new WebAPIClient(buri, headers);
                },
                baseUri,
                headersHandle,
                identifier
                );
        }

        [ExcelFunction(
            Name = prefixFunctionName + nameof(WebAPIGet),
            Category = categoryName)]
        public static object WebAPIGet(string clientHandle, string requestUri, object identifier)
        {
            var context = ExLibrisContext.DefaultContext;
            var support = context.GetFunctionCallSupport();

            return ExLibrisUtility.ExcelObserveObjectRegistrationAsync(
                nameof(WebAPIGet),
                support.ObjectRepository,
                () =>
                {
                    var webapi = support.ObjectRepository.GetObject<WebAPIClient>(clientHandle);

                    if (ExLibrisUtility.IsExcelError(identifier))
                    {
                        throw new ArgumentException($"{nameof(identifier)} is Error");
                    }

                    var ru = support.ToValueAsString(requestUri);

                    if (string.IsNullOrWhiteSpace(ru))
                    {
                        return webapi.Get();
                    }
                    else
                    {
                        return webapi.Get(ru);
                    }
                },
                clientHandle,
                requestUri,
                identifier
                );
        }

        [ExcelFunction(
            Name = prefixFunctionName + nameof(WebAPIPost),
            Category = categoryName)]
        public static object WebAPIPost(string clientHandle, string requestUri, object requestContent, bool formContent, object identifier)
        {
            var context = ExLibrisContext.DefaultContext;
            var support = context.GetFunctionCallSupport();

            return ExLibrisUtility.ExcelObserveObjectRegistrationAsync(
                nameof(WebAPIPost),
                support.ObjectRepository,
                () =>
                {
                    var webapi = support.ObjectRepository.GetObject<WebAPIClient>(clientHandle);

                    if (ExLibrisUtility.IsExcelError(identifier))
                    {
                        throw new ArgumentException($"{nameof(identifier)} is Error");
                    }

                    var ru = support.ToValueAsString(requestUri);

                    if (formContent)
                    {
                        var content = WebAPIFunctions.ConvertParamters(requestContent, support);
                        return string.IsNullOrEmpty(ru) ?
                            webapi.Post(content) :
                            webapi.Post(ru, content);
                    }
                    else
                    {
                        var content = (string)support.ToValue(requestContent);
                        return string.IsNullOrEmpty(ru) ?
                            webapi.Post(content) :
                            webapi.Post(ru, content);
                    }

                },
                clientHandle,
                requestUri,
                requestContent,
                formContent,
                identifier
                );
        }

    }
}
