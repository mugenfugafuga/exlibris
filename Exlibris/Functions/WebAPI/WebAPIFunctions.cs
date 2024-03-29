﻿using ExcelDna.Integration;
using Exlibris.Core.WebAPI;
using Exlibris.Excel;
using Exlibris.Functions.JSON;
using Exlibris.Functions.XML;
using Newtonsoft.Json.Linq;
using System.Linq;
using System;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;

namespace Exlibris.Functions.WebAPI
{
    public static partial class WebAPIFunctions
    {
        private const string Category = "Exlibris.WebAPI";

        private static HttpContent CreateHttpContent(ExlibrisExcelFunctionSupport support, IExcelValue data, IScalar contentType)
        {
            if (data.DataType == ExcelDataType.Scalar)
            {
                return CreateHttpContentFromScalar(support, data.ShouldBeScalar(), contentType);
            }
            else
            {
                // data.DataType = ExcelDataType.Matrix
                var formdata = data.ShouldBeMatrix().Rows.ToDictionary(r => r[0].GetValueOrThrow<string>(), r => r[1].GetValueOrThrow<string>());

                var content = new FormUrlEncodedContent(formdata);
                content.Headers.ContentType = new MediaTypeHeaderValue(contentType.GetValueOrDefault("application/x-www-form-urlencoded"));
                return content;
            }
        }

        private static HttpContent CreateHttpContentFromScalar(ExlibrisExcelFunctionSupport support, IScalar json, IScalar contentType)
        {
            if (json.TryGetValue<JToken>(out var jo))
            {
                return new StringContent(support.JSONSerializer.Serialize(jo), Encoding.UTF8, contentType.GetValueOrDefault("application/json"));
            }

            return new StringContent(json.GetValueOrThrow<string>(), Encoding.UTF8, contentType.GetValueOrDefault("text/plain"));
        }

        private static async Task PostProcessingAsync(ExlibrisExcelFunctionSupport support, IExcelObserver observer, HttpResponseMessage response)
        {
            if (response?.Content != null)
            {
                var content = response.Content;

                switch (content.Headers.ContentType?.MediaType)
                {
                    case "application/json":
                        observer.OnNext(support.JSONSerializer.Deserialize(await response.Content.ReadAsStringAsync()));
                        return;
                    case "application/xml":
                        observer.OnNext(XMLFuncUtil.CreateXMLObjectFromString(await response.Content.ReadAsStringAsync()));
                        return;
                    case "text/plain":
                        observer.OnNext(await response.Content.ReadAsStringAsync());
                        return;
                    default:
                        var message = await response.Content.ReadAsStringAsync();

                        try
                        {
                            observer.OnNext(support.JSONSerializer.Deserialize(message));
                        }
                        catch (Exception)
                        {
                            observer.OnNext(message);
                        }
                        return;
                }
            }
            else
            {
                observer.OnError(new InvalidOperationException($"no content"));
            }
        }

        private static HttpRequestMessage CreateRequest(HttpMethod method, string url, IExcelValue param, ExlibrisExcelFunctionSupport support)
            => CreateRequest(method, url, param, null, support);

        private static HttpRequestMessage CreateRequest(HttpMethod method, string url, IExcelValue param, HttpContent content, ExlibrisExcelFunctionSupport support)
        {
            if (param.IfScalar()?.TryGetValue<AuthenticationHeaderValue>(out var h) ?? false)
            {
                return WebAPIUtil.CreateRequest(method, url, h, content);
            }

            var json = JSONFuncUtil.CreateJSONObject(support, param);

            if (json.Type == JTokenType.Null)
            {
                return WebAPIUtil.CreateRequest(method, url, content);
            }

            var setting = support.JSONSerializer.ToObject<HttpRequestSetting>(json) ??
                throw new ArgumentException($"cannot convert to {typeof(HttpRequestMessage)}. json : {json}");

            return WebAPIUtil.CreateRequest(method, url, setting, content);
        }
    }
}