using ExcelDna.Integration;
using ExLibris.Core;
using ExLibris.Core.WebAPIs;
using ExLibris.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ExLibris.WebAPIs
{
    public static class WebAPIFunctions
    {
        [ExcelFunction(
            Name = "ExLibris.WebAPIs.OpenWebSocket",
            Category = "ExLibris.WebAPIs")]
        public static object OpenWebAPI(string baseUri, object identifier)
        {
            var context = ExLibrisContext.DefaultContext;
            var support = context.GetFunctionCallSupport();

            return ExLibrisUtility.ExcelObserveObjectRegistration(
                nameof(OpenWebAPI),
                support.ObjectRepository,
                () =>
                {
                    if (ExLibrisUtility.IsExcelError(identifier))
                    {
                        throw new ArgumentException($"{nameof(identifier)} is Error");
                    }

                    var bu = support.ToValueAsString(baseUri);

                    if (string.IsNullOrWhiteSpace(bu))
                    {
                        return new WebAPI();
                    }
                    else
                    {
                        return new WebAPI(bu);
                    }
                },
                baseUri,
                identifier
                );
        }

        [ExcelFunction(
            Name = "ExLibris.WebAPIs.WebAPIGet",
            Category = "ExLibris.WebAPIs")]
        public static object WebAPIGet(string webAPIHandle, string requestUri, object identifier)
        {
            var context = ExLibrisContext.DefaultContext;
            var support = context.GetFunctionCallSupport();

            return ExLibrisUtility.ExcelObserveObjectRegistration(
                nameof(WebAPIGet),
                support.ObjectRepository,
                () =>
                {
                    var webapi = support.ObjectRepository.GetObject<WebAPI>(webAPIHandle);

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
                webAPIHandle,
                requestUri,
                identifier
                );
        }

        [ExcelFunction(
            Name = "ExLibris.WebAPIs.WebAPIPost",
            Category = "ExLibris.WebAPIs")]
        public static object WebAPIPost(string webAPIHandle, string requestUri, object requestContent, bool formContent, object identifier)
        {
            var context = ExLibrisContext.DefaultContext;
            var support = context.GetFunctionCallSupport();

            return ExLibrisUtility.ExcelObserveObjectRegistration(
                nameof(WebAPIPost),
                support.ObjectRepository,
                () =>
                {
                    var webapi = support.ObjectRepository.GetObject<WebAPI>(webAPIHandle);

                    if (ExLibrisUtility.IsExcelError(identifier))
                    {
                        throw new ArgumentException($"{nameof(identifier)} is Error");
                    }

                    var ru = support.ToValueAsString(requestUri);

                    if (formContent)
                    {
                        var content = GetParamters(support.ToValue(requestContent), support);
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
                webAPIHandle,
                requestUri,
                requestContent,
                formContent,
                identifier
                );
        }

        [ExcelFunction(
            Name = "ExLibris.WebAPIs.BuildUri",
            Category = "ExLibris.WebAPIs")]
        public static object BuildUri(string requestUri, object parameters)
        {
            var context = ExLibrisContext.DefaultContext;
            var support = context.GetFunctionCallSupport();

            return ExLibrisUtility.FuncOrNAIfThrown(
                () =>
                {
                    var ru = support.ToValueAsString(requestUri);
                    var p = GetParamters(support.ToValue(parameters), support);

                    return WebAPI.ResolveUri(ru, p);
                }
                );
        }

        private static Dictionary<string, string> GetParamters(object parameters, ExcelFunctionCallSupport support)
        {
            try
            {
               if(parameters == null)
                {
                    return new Dictionary<string, string>();
                }

                var jo = parameters is object[,] matrix ?
                    JsonFunctions.CreateJsonObjectByMatrix(support.GetExcelMatrixAccessor(matrix), support) :
                    support.ObjectRepository.GetObject((string)parameters);

                return support.NewJsonObjectAccessor(jo)
                    .GetJsonValues()
                    .ToDictionary(kv => kv.KeyPath ?? string.Empty, kv => kv.Value?.ToString() ?? string.Empty);
            }
            catch(Exception e)
            {
                throw new Exception($"can't resolve paramters paramters : {parameters}", e);
            }

        }

        [ExcelFunction(
            Name = "ExLibris.WebAPIs.WebAPIRequestContent",
            Category = "ExLibris.WebAPIs")]
        public static object WebAPIResponseContent(string responseHandle)
        {
            var context = ExLibrisContext.DefaultContext;
            var support = context.GetFunctionCallSupport();

            return ExLibrisUtility.RunAsync(
                nameof(WebAPIResponseContent),
                () =>
                {
                    var response = support.ObjectRepository.GetObject<Response>(responseHandle);

                    return response.GetContent();
                },
                responseHandle
                );
        }

        [ExcelFunction(
            Name = "ExLibris.WebAPIs.WebAPIResponseStatus",
            Category = "ExLibris.WebAPIs")]
        public static object WebAPIResponseStatus(string responseHandle, bool codeOnly = false)
        {
            var context = ExLibrisContext.DefaultContext;
            var support = context.GetFunctionCallSupport();

            return ExLibrisUtility.RunAsync(
                nameof(WebAPIResponseStatus),
                () =>
                {
                    var response = support.ObjectRepository.GetObject<Response>(support.ToValueAsString(responseHandle));

                    return codeOnly ? (object)response.HttpStatusCode : response.HttpStatus;
                },
                responseHandle,
                codeOnly
                );
        }

        [ExcelFunction(
            Name = "ExLibris.WebAPIs.WebAPIResponseHeader",
            Category = "ExLibris.WebAPIs")]
        public static object WebAPIResponseHeader(string responseHandle)
        {
            var context = ExLibrisContext.DefaultContext;
            var support = context.GetFunctionCallSupport();

            return ExLibrisUtility.RunAsync(
                nameof(WebAPIResponseHeader),
                () =>
                {
                    var response = support.ObjectRepository.GetObject<Response>(support.ToValueAsString(responseHandle));

                    return response.Headers;
                },
                responseHandle
                );
        }
    }
}
