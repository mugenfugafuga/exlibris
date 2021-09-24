using ExcelDna.Integration;
using ExLibris.Core;
using ExLibris.Core.WebAPIs;
using ExLibris.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ExLibris.WebAPIs.Client
{
    public static class WebAPIClientFunctions
    {
        [ExcelFunction(
            Name = "ExLibris.WebAPIs.Client.CreateWebAPIClient",
            Category = "ExLibris.WebAPIs")]
        public static object CreateWebAPIClient(string baseUri, string headersHandle, object identifier)
        {
            var context = ExLibrisContext.DefaultContext;
            var support = context.GetFunctionCallSupport();

            return ExLibrisUtility.ExcelObserveObjectRegistration(
                nameof(CreateWebAPIClient),
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
            Name = "ExLibris.WebAPIs.Client.WebAPIGet",
            Category = "ExLibris.WebAPIs")]
        public static object WebAPIGet(string clientHandle, string requestUri, object identifier)
        {
            var context = ExLibrisContext.DefaultContext;
            var support = context.GetFunctionCallSupport();

            return ExLibrisUtility.ExcelObserveObjectRegistration(
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
            Name = "ExLibris.WebAPIs.Client.WebAPIPost",
            Category = "ExLibris.WebAPIs")]
        public static object WebAPIPost(string clientHandle, string requestUri, object requestContent, bool formContent, object identifier)
        {
            var context = ExLibrisContext.DefaultContext;
            var support = context.GetFunctionCallSupport();

            return ExLibrisUtility.ExcelObserveObjectRegistration(
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
