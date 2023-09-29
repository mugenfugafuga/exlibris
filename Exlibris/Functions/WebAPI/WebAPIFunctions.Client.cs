using ExcelDna.Integration;
using Exlibris.Core.WebAPI;
using Exlibris.Functions.JSON;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;

namespace Exlibris.Functions.WebAPI
{
    partial class WebAPIFunctions
    {
        [ExcelFunction(
            Category = Category,
            Name = Category + ".Client",
            Description = "generates a http client in memory.")]
        public static object Client(
            [ExcelArgument(AllowReference = true, Description = "json string, file path or JSONPath and Value Pairs.")] object source,
            [ExcelArgument(Description = "optional argument.if the argument is invalid, the method will not execute the process.")] object identifier
            )
            => ExlibrisAddin.GetFunctionSupport().NoError(identifier).ErrorValueIfThrown(support =>
            {
                var src = support.NotDateTime(source, nameof(source));

                return support.ObserveObjectRegistration(
                    () =>
                    {
                        var json = JSONFuncUtil.CreateJSONObject(support, src);

                        if (json.Type == JTokenType.Null)
                        {
                            return WebAPIUtil.NewClinet();
                        }

                        var setting = support.JSONSerializer.ToObject<HttpClientSetting>(json) ??
                            throw new ArgumentException($"cannot convert to {typeof(HttpClient)}. json : {json}");

                        return WebAPIUtil.NewClinet(setting);

                    }, source, identifier);
            });
    }
}