using ExcelDna.Integration;
using Exlibris.Core.Reflection;
using Exlibris.Excel;
using System;

namespace Exlibris.Functions.JSON
{
    partial class JSONFunctions
    {
        [ExcelFunction(
            Category = Category,
            Name = Category + ".JSONObject",
            Description = "generates a JSON management object in memory.")]
        public static object JSONObject(
            [ExcelArgument(AllowReference = true, Description = "json string, file path or JSONPath and Value Pairs.")] object source,
            [ExcelArgument(AllowReference = true, Description = "optional argument. select the DateTimeDetector")] object dateTimeDetection,
            [ExcelArgument(AllowReference = true, Description = "optional argument. C# class name. convert to the specified class.")] object className,
            [ExcelArgument(Description = "optional argument.if the argument is invalid, the method will not execute the process.")] object identifier
            )
            => ExlibrisAddin.GetFunctionSupport().NoError(identifier).ErrorValueIfThrown(support =>
            {
                var dtd = support.NotDateTime(dateTimeDetection, nameof(dateTimeDetection)).TryGetValue<DateTimeDetector>(out var d) ? d : null;
                var src = support.NewExcelValue(source, nameof(source), dtd);
                var classNameVal = support.NotDateTime(className, nameof(className)).ShouldBeScalar();

                return support.ObserveObjectRegistration(
                    () =>
                    {
                        var json = JSONFuncUtil.CreateJSONObject(support, src);

                        if (classNameVal.TryGetValue<string>(out var cn))
                        {
                            var type = ReflectionUtil.GetType(cn);
                            return support.JSONSerializer.ToObject(json, type) ?? throw new ArgumentException($"can not convert to {type}");
                        }
                        else
                        {
                            return json;
                        }
                    }, source, className, identifier);
            });
    }
}