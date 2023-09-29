using ExcelDna.Integration;
using Exlibris.Core.JSONs;
using Exlibris.Core.JSONs.Extension;
using Newtonsoft.Json.Linq;
using System.Linq;
using System;

namespace Exlibris.Functions.JSON
{
    partial class JSONFunctions
    {
        [ExcelFunction(
            Category = Category,
            Name = Category + ".Table",
            Description = "displays a JSON array in a Table structure. displays json paths as field names in the first row.")]
        public static object Table(
            [ExcelArgument(AllowReference = true, Description = "JSON management object.")] object json,
            [ExcelArgument(AllowReference = true, Description = "optional argument. specifies the JSON Path of the element to display. If specified, only the element corresponding to the JSON Path will be displayed. if not specified, displays all JSON Paths.")] object paths,
            [ExcelArgument(AllowReference = true, Description = "optional argument. specifies whether to display the root of the JSON Path ('$' symbol) or not. If set to true, the root will be displayed. If set to false, the root will not be displayed. The default value is false.")] object pathWithRoot,
            [ExcelArgument(AllowReference = true, Description = "optional argument. specifies whether to display the field names in the table data or not. If set to true, the field names will be displayed. If set to false, only the values will be displayed without the field names. The default value is true.")] object withFieldName,
            [ExcelArgument(Description = "optional argument.if the argument is invalid, the method will not execute the process.")] object identifier)
            => ExlibrisAddin.GetFunctionSupport().NoError(identifier).ErrorValueIfThrown(support =>
            {
                var jsonVal = support.NotDateTime(json, nameof(json)).ShouldBeScalar().GetValueOrThrow<JToken>();
                var serializer = support.JSONSerializer;

                var defaultpaths = support.NotDateTime(paths, nameof(paths)).Values
                    .Where(v => v.Value != null)
                    .Select(v => v.GetValueOrThrow<string>())
                    .ToArray();

                var pwr = support.NotDateTime(pathWithRoot, nameof(pathWithRoot)).ShouldBeScalar().GetValueOrDefault(true);
                Func<string, string> pathfunc;
                if (pwr)
                {
                    pathfunc = JSONUtil.PathWithoutRoot;
                }
                else
                {
                    pathfunc = p => p;
                }

                var withField = support.NotDateTime(withFieldName, nameof(withFieldName)).ShouldBeScalar().GetValueOrDefault(true);

                return support.Observe(false, (_0, _1) =>
                {
                    if (serializer.TryGetJSONArray(jsonVal, out var array))
                    {
                        return BuildTable(array, defaultpaths, withField, pathfunc, support);
                    }

                    if (serializer.TryGetJSONObject(jsonVal, out var jobject))
                    {
                        return BuildTable(jobject, defaultpaths, withField, pathfunc, support);
                    }

                    return jsonVal.GetValueOrThis();
                }, json, paths, pathWithRoot, withFieldName, identifier);

            });

        private static object BuildTable(
            JArray array,
            string[] defaultpaths,
            bool withFieldName,
            Func<string, string> pathConverter,
            ExlibrisExcelFunctionSupport support)
        {
            var serializer = support.JSONSerializer;
            var elems = serializer.FilterJsons(array, "[*]").Select(v => v.Value).ToArray();
            var paths = defaultpaths.Length == 0 ?
                elems.SelectMany(e => serializer.GetValues(e)).Select(v => v.Path).Distinct().ToArray() :
                defaultpaths;

            var builder = support.NewMatrixBuilder();

            if (withFieldName)
            {
                paths.Select(pathConverter).Aggregate(builder.NewRow(), (row, path) => row.Add(path)).Close();
            }

            foreach (var elem in elems)
            {
                paths
                    .Select(path => serializer.FilterJson(elem, path).Value)
                    .Aggregate(builder.NewRow(), (row, value) => row.Add(value?.GetValueOrThis()))
                    .Close();
            }

            return builder.Build();
        }

        private static object BuildTable(
            JObject jobject,
            string[] defaultpaths,
            bool withFieldName,
            Func<string, string> pathConverter,
            ExlibrisExcelFunctionSupport support)
        {
            var serializer = support.JSONSerializer;

            var paths = defaultpaths.Length == 0 ?
                serializer.GetValues(jobject).Select(v => v.Path).ToArray() :
                defaultpaths;

            var builder = support.NewMatrixBuilder();

            if (withFieldName)
            {
                paths.Aggregate(builder.NewRow(), (row, path) => row.Add(pathConverter(path))).Close();
            }

            var valuesrow = builder.NewRow();

            foreach (var path in paths)
            {
                var pathValue = serializer.FilterJson(jobject, path);
                var jt = pathValue.Value;

                valuesrow.Add(jt?.GetValueOrThis());
            }

            return builder.Build();
        }
    }
}