using ExcelDna.Integration;
using ExLibris.Core;
using ExLibris.Core.WebAPIs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ExLibris.WebAPIs
{
    public static class WebAPIFunctions
    {
        static WebAPIFunctions()
        {
            ExLibrisContext.SecurityProtocolUpdateFunction.Invoke();
        }

        private const string categoryName = "ExLibris.WebAPIs";
        private const string prefixFunctionName = categoryName + ".";

        [ExcelFunction(
            Name = prefixFunctionName + nameof(CreateHeaders),
            Category = categoryName)]
        public static object CreateHeaders(object[,] matrix)
        {
            var context = ExLibrisContext.DefaultContext;
            var support = context.GetFunctionCallSupport();

            return ExLibrisUtility.ExcelObserveObjectRegistration(
                nameof(CreateHeaders),
                support.ObjectRepository,
                () =>
                {
                    var jo = support.CreateJsonObject(matrix);
                    return Core.Json.JsonObjectSerialiser.ToObject<WebAPIHeaders>(jo);
                },
                matrix
                );
        }

        [ExcelFunction(
            Name = prefixFunctionName + nameof(CreateHeadersAsync),
            Category = categoryName)]
        public static object CreateHeadersAsync(object[,] matrix)
        {
            var context = ExLibrisContext.DefaultContext;
            var support = context.GetFunctionCallSupport();

            return ExLibrisUtility.ExcelObserveObjectRegistrationAsync(
                nameof(CreateHeadersAsync),
                support.ObjectRepository,
                () =>
                {
                    var jo = support.CreateJsonObject(matrix);
                    return Core.Json.JsonObjectSerialiser.ToObject<WebAPIHeaders>(jo);
                },
                matrix
                );
        }

        [ExcelFunction(
            Name = prefixFunctionName + nameof(BuildUri),
            Category = categoryName)]
        public static object BuildUri(string requestUri, object parameters)
        {
            var context = ExLibrisContext.DefaultContext;
            var support = context.GetFunctionCallSupport();

            return ExLibrisUtility.FuncOrNAIfThrown(
                () =>
                {
                    var ru = support.ToValueAsString(requestUri);
                    var p = support.ConvertKeyValues(parameters).ToDictionary(kv => kv.Key, kv => kv.Value);

                    return WebAPIUtility.ResolveUri(ru, p);
                }
                );
        }

        internal static Dictionary<string, string> ConvertParamters(object parameters, ExcelFunctionCallSupport support)
        {
            try
            {
                var param = support.ToValue(parameters);

                if (param == null)
                {
                    return new Dictionary<string, string>();
                }

                var jo = param is object[,] matrix ?
                    support.CreateJsonObject(matrix) :
                    support.ObjectRepository.GetObject((string)param);

                return support.NewJsonObjectAccessor(jo)
                    .GetJsonValues()
                    .ToDictionary(kv => kv.KeyPath ?? string.Empty, kv => kv.Value?.ToString() ?? string.Empty);
            }
            catch (Exception e)
            {
                throw new Exception($"can't resolve paramters paramters : {parameters}", e);
            }

        }

        [ExcelFunction(
            Name = prefixFunctionName + nameof(WebAPIResponseContent),
            Category = categoryName)]
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
            Name = prefixFunctionName + nameof(WebAPIResponseStatus),
            Category = categoryName)]
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
            Name = prefixFunctionName + nameof(WebAPIResponseHeader),
            Category = categoryName)]
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

        [ExcelFunction(
           Name = prefixFunctionName + "GET",
           Category = categoryName)]
        public static object WebAPIGet(string requestUri, string headersHandle = null)
        {
            var context = ExLibrisContext.DefaultContext;
            var support = context.GetFunctionCallSupport();

            return ExLibrisUtility.ExcelObserveObjectRegistrationAsync(
                nameof(WebAPIGet),
                support.ObjectRepository,
                () =>
                {
                    var headers = string.IsNullOrEmpty(headersHandle) ?
                        null :
                        support.ObjectRepository.GetObject<WebAPIHeaders>(headersHandle);

                    var ru = support.ToValueAsString(requestUri);

                    using (var client = new WebAPIClient(null, headers))
                    {
                        return client.Get(ru);
                    }
                },
                requestUri,
                headersHandle
                );
        }

        [ExcelFunction(
           Name = prefixFunctionName + "POST",
           Category = categoryName)]
        public static object WebAPIPost(string requestUri, object requestContent, bool formContent = false, string headersHandle = null)
        {
            var context = ExLibrisContext.DefaultContext;
            var support = context.GetFunctionCallSupport();

            return ExLibrisUtility.ExcelObserveObjectRegistrationAsync(
                nameof(WebAPIPost),
                support.ObjectRepository,
                () =>
                {
                    var headers = string.IsNullOrEmpty(headersHandle) ?
                        null :
                        support.ObjectRepository.GetObject<WebAPIHeaders>(headersHandle);

                    var ru = support.ToValueAsString(requestUri);

                    using (var client = new WebAPIClient(null, headers))
                    {
                        if (formContent)
                        {
                            var content = ConvertParamters(requestContent, support);
                            return client.Post(ru, content);
                        }
                        else
                        {
                            var content = (string)support.ToValue(requestContent);
                            return client.Post(ru, content);
                        }
                    }
                },
                requestUri,
                requestContent,
                formContent,
                headersHandle
                );
        }

        [ExcelFunction(
            Name = prefixFunctionName + nameof(CreateFormRequest),
            Category = categoryName)]
        public static object CreateFormRequest(object parameter)
        {
            var context = ExLibrisContext.DefaultContext;
            var support = context.GetFunctionCallSupport();

            return ExLibrisUtility.ExcelObserveObjectRegistration(
                nameof(CreateFormRequest),
                support.ObjectRepository,
                () => ConvertFormRequest(parameter, support),
                parameter
                );
        }

        [ExcelFunction(
            Name = prefixFunctionName + nameof(CreateFormRequestAsync),
            Category = categoryName)]
        public static object CreateFormRequestAsync(object parameter)
        {
            var context = ExLibrisContext.DefaultContext;
            var support = context.GetFunctionCallSupport();

            return ExLibrisUtility.ExcelObserveObjectRegistrationAsync(
                nameof(CreateFormRequestAsync),
                support.ObjectRepository,
                () => ConvertFormRequest(parameter, support),
                parameter
                );
        }

        private static FormRequest ConvertFormRequest(object parameter, ExcelFunctionCallSupport support)
        {
            var kvs = support.ConvertKeyValues(parameter);

            return new FormRequest
            {
                Values = kvs.Select(kv => new FormValue { Name = kv.Key, Value = kv.Value,}).ToList(),
            };
        }
    }
}
