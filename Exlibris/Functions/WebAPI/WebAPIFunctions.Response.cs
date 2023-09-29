using ExcelDna.Integration;
using System;
using System.Net.Http;

namespace Exlibris.Functions.WebAPI
{
    partial class WebAPIFunctions
    {
        [ExcelFunction(
            Category = Category,
            Name = Category + ".Response",
            Description = "http response.",
            IsHidden = true)]
        public static object Response(
            [ExcelArgument(AllowReference = true, Description = "target cell")] object targetCell,
            [ExcelArgument(Description = "optional argument.if the argument is invalid, the method will not execute the process.")] object identifier)
            => ExlibrisAddin.GetFunctionSupport().NoError(identifier).ErrorValueIfThrown(support =>
            {
                var address = ExlibrisUtil.GetRawExcel(targetCell, nameof(targetCell)).Address ?? throw new ArgumentException($"{nameof(targetCell)} is not cell.");

                return support.ObserveObjectRegistration(() =>
                {
                    return support.JSONSerializer.FromObject(support.GetCachedObject<HttpResponseMessage>(address));
                }, targetCell, identifier);
            });
    }
}