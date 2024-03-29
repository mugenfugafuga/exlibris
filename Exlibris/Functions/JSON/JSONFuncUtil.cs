﻿using Exlibris.Excel;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Xml;

namespace Exlibris.Functions.JSON
{
    static class JSONFuncUtil
    {
        public static JToken CreateJSONObject(ExlibrisExcelFunctionSupport support, IExcelValue value)
        {
            var serializer = support.JSONSerializer;

            switch (value.DataType)
            {
                case ExcelDataType.Matrix:
                    return CreateJSONObjectFromMatrix(support, value.ShouldBeMatrix());

                case ExcelDataType.Scalar:
                    {
                        if (value.TryGetValue<string>(out var vs))
                        {
                            return CreateJSONObjectFromString(support, vs);
                        }
                        else if (value.TryGetValue<XmlDocument>(out var xml))
                        {
                            return serializer.FromXml(xml);
                        }
                        else
                        {
                            // try to generate JSON management object from C# object;
                            return serializer.FromObject(value.Value);
                        }

                    }

                default: throw new NotImplementedException(value.DataType.ToString());
            }
        }

        public static JToken CreateJSONObjectFromMatrix(ExlibrisExcelFunctionSupport support, IMatrix matrix)
        {
            var builder = support.JSONSerializer.NewJSONBuilder();

            foreach (var row in matrix.Rows)
            {
                var path = row[0].GetValueOrThrow<string>();
                var val = row[1].Value;

                builder.Append(path, val);
            }

            return builder.Build();
        }

        public static JToken CreateJSONObjectFromString(ExlibrisExcelFunctionSupport support, string value)
        {
            if (File.Exists(value))
            {
                return support.JSONSerializer.LoadFile(value);
            }

            if (ExlibrisUtil.TryGet(() => support.JSONSerializer.Deserialize(value), out var jt))
            {
                return jt;
            }

            return support.JSONSerializer.FromObject(value);
        }
    }
}