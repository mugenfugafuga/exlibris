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
            Name = "ExLibris.WebAPIs.CreateHeaders",
            Category = "ExLibris.WebAPIs")]
        public static object CreateHeaders(object[,] matrix)
        {
            var context = ExLibrisContext.DefaultContext;
            var support = context.GetFunctionCallSupport();

            return ExLibrisUtility.ExcelObserveObjectRegistration(
                nameof(CreateHeaders),
                support.ObjectRepository,
                () =>
                {
                    var jo = JsonFunctions.CreateJsonObjectByMatrix(support.GetExcelMatrixAccessor(matrix), support);
                    return Core.Json.JsonObjectSerialiser.ToObject<WebAPIHeaders>(jo);
                },
                matrix
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
                    var p = ConvertParamters(parameters, support);

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
                    JsonFunctions.CreateJsonObjectByMatrix(support.GetExcelMatrixAccessor(matrix), support) :
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
