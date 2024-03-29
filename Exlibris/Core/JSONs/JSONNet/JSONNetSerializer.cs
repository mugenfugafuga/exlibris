﻿using Exlibris.Core.JSONs.JSONNet;
using Exlibris.Core.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Schema.Generation;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace Exlibris.Core.JSONs.JsonNet
{
    public class JSONNetSerializer : IJSONSerializer
    {
        private readonly ConcurrentDictionary<string, JSchema> jsonSchemas = new ConcurrentDictionary<string, JSchema>();
        private readonly JSchemaGenerator schemaGenerator;
        private readonly JsonSerializer noIndentedJsonSerializer;
        private readonly JsonSerializer indentedJsonSerializer;
        private readonly JsonSerializerSettings jsonSerializerSettings;

        public JSONNetSerializer(IEnumerable<JsonConverter> jsonConverters, IEnumerable<JSchemaGenerationProvider> jSchemaGenerationProviders)
        {
            schemaGenerator = new JSchemaGenerator();
            foreach (var jSchemaGenerationProvider in jSchemaGenerationProviders)
            {
                schemaGenerator.GenerationProviders.Add(jSchemaGenerationProvider);
            }

            var converters = jsonConverters.ToArray();

            jsonSerializerSettings = new JsonSerializerSettings
            {
                Converters = converters,
            };

            noIndentedJsonSerializer = JsonSerializer.Create(jsonSerializerSettings);

            var indented = new JsonSerializerSettings
            {
                Converters = converters,
                Formatting = Newtonsoft.Json.Formatting.Indented,
            };
            indentedJsonSerializer = JsonSerializer.Create(indented);
        }

        private JSchema GetJSchema(string typeName)
            => jsonSchemas.GetOrAdd(typeName, (_) =>
            {
                var type = ReflectionUtil.GetType(typeName);
                return schemaGenerator.Generate(type);
            });

        private static Func<string, string> GetWithRootFunction(JToken json)
        {
            switch (json.Type)
            {
                case JTokenType.Object: return (string path) => string.IsNullOrEmpty(path) ? "$" : $"$.{path}";
                case JTokenType.Array: return (string path) => $"${path}";
                default: return (string path) => "$";
            }
    }
          

        public object ToObject(JToken json, Type objectType) => json.ToObject(objectType);

        public T ToObject<T>(JToken json) => json.ToObject<T>();

        public JToken FromObject(object obj) => obj == null ? JValue.CreateNull() : JToken.FromObject(obj);

        public JToken LoadFile(string filePath)
        {
            using (var textReader = File.OpenText(filePath))
            {
                using (var jsonReader = new JsonTextReader(textReader))
                {
                    return JToken.Load(jsonReader);
                }
            }
        }

        public void SaveFile(JToken json, string filePath, bool pretty)
        {
            using (var file = new StreamWriter(filePath))
            {
                if (pretty)
                {
                    indentedJsonSerializer.Serialize(file, json);
                }
                else
                {
                    noIndentedJsonSerializer.Serialize(file, json);
                }
            }
        }

        public string Serialize(JToken json) => JsonConvert.SerializeObject(json, jsonSerializerSettings);

        public JToken Deserialize(string jsonString)
            => JsonConvert.DeserializeObject<JToken>(jsonString, jsonSerializerSettings) is JToken jt
                ? jt
                : throw new ArgumentException($"can not parse Json String. {jsonString}");

        public (string Path, JToken Value) FilterJson(JToken json, string jsonPath)
        {
            var withRoot = GetWithRootFunction(json);
            var je = json.SelectToken(jsonPath);

            if (je == null)
            {
                return (null, null);
            }

            return (withRoot(je.Path), je.DeepClone());
        }

        public IEnumerable<(string Path, JToken Value)> FilterJsons(JToken json, string jsonPath)
        {
            var withRoot = GetWithRootFunction(json);

            return json.SelectTokens(jsonPath).Select(je => (withRoot(je.Path), je.DeepClone()));
        }

        public IEnumerable<(string Path, JValue Value)> GetValues(JToken json)
        {
            var withRoot = GetWithRootFunction(json);
            return json.SelectTokens("..*").OfType<JValue>().Select(je => (withRoot(je.Path), je));
        }

        public bool IsJSONObject(JToken json) => json is JObject;

        public bool IsJSONArray(JToken json) => json is JArray;

        public JToken GetJSONOrDefault(object value)
        {
            if (value is JToken token)
            {
                return token;
            }

            return null;
        }

        public bool TryGetJSONObject(JToken json, out JObject jsonObject)
        {
            if (json is JObject jo)
            {
                jsonObject = jo;
                return true;
            }

            jsonObject = default;
            return false;
        }

        public bool TryGetJSONArray(JToken json, out JArray jsonArray)
        {
            if (json is JArray ja)
            {
                jsonArray = ja;
                return true;
            }

            jsonArray = default;
            return false;
        }

        public string SerializeSchema(JSchema jsonSchema) => JsonConvert.SerializeObject(jsonSchema, jsonSerializerSettings);

        public JSchema DeserializeSchema(string jsonString) => JSchema.Parse(jsonString);

        public JSchema GetSchema(string typeName) => GetJSchema(typeName);

        public JSchema LoadSchemaFile(string filePath)
        {
            using (var file = new StreamReader(filePath))
            {
                using (var json = new JsonTextReader(file))
                {
                    return JSchema.Load(json);
                }
            }
        }

        public void SaveSchemaFile(JSchema jsonSchema, string filePath, bool pretty)
        {
            using (var file = new StreamWriter(filePath))
            {
                if (pretty)
                {
                    indentedJsonSerializer.Serialize(file, jsonSchema);
                }
                else
                {
                    noIndentedJsonSerializer.Serialize(file, jsonSchema);
                }
            }
        }

        public XmlDocument ToXml(JToken json)
        {
            try
            {
                return JsonConvert.DeserializeXmlNode(Serialize(json)) ?? throw new ArgumentException($"cannot convert to Xml. json : {json}");
            }
            catch (Exception)
            {
                return JsonConvert.DeserializeXmlNode(Serialize(json), "xml") ?? throw new ArgumentException($"cannot convert to Xml. json : {json}");
            }
        }

        public JToken FromXml(XmlDocument xml)
            => JToken.Parse(JsonConvert.SerializeXmlNode(xml));

        public IJSONBuilder<JToken> NewJSONBuilder() => new JSONObjectBuilder();
    }
}