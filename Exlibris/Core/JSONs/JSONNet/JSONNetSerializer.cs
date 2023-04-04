using Exlibris.Core.JSONs.JSONNet;
using Exlibris.Core.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Schema.Generation;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace Exlibris.Core.JSONs.JsonNet;

internal class JSONNetSerializer : IJSONSerializer<JObject, JArray,JValue, JToken, JSchema>
{
    private static readonly ConcurrentDictionary<string, JSchema> jsonSchemas = new();
    private static readonly JSchemaGenerator schemaGenerator = new();
    private static readonly JsonSerializer noIndentedJsonSerializer;
    private static readonly JsonSerializer indentedJsonSerializer;

    static JSONNetSerializer()
    {
        JsonConvert.DefaultSettings = () => new JsonSerializerSettings
        {
            Converters = new[] { new StringEnumConverter(), },
        };

        noIndentedJsonSerializer = JsonSerializer.Create();

        var indented = JsonConvert.DefaultSettings();
        indented.Formatting = Formatting.Indented;

        indentedJsonSerializer = JsonSerializer.Create(indented);
    }

    public static JSchema GetJSchema(string typeName)
        => jsonSchemas.GetOrAdd(typeName, (typeName) =>
        {
            var type = ReflectionUtil.GetType(typeName);
            return schemaGenerator.Generate(type);
        });

    private static Func<string, string> GetWithRootFunction(JToken json)
        => json.Type switch
        {
            JTokenType.Object => (string path) => string.IsNullOrEmpty(path) ? "$" : $"$.{path}",
            JTokenType.Array => (string path) => $"${path}",
            _ => (string path) => "$",
        };

    public object? ToObject(JToken json, Type objectType) => json.ToObject(objectType);

    public JToken FromObject(object? obj) => obj == null ? JValue.CreateNull() : JToken.FromObject(obj);

    public JToken LoadFile(string filePath)
    {
        using var textReader = File.OpenText(filePath);
        using var jsonReader = new JsonTextReader(textReader);

        return JToken.Load(jsonReader);
    }

    public void SaveFile(JToken json, string filePath, bool pretty)
    {
        using var file = new StreamWriter(filePath);

        if (pretty)
        {
            indentedJsonSerializer.Serialize(file, json);
        }
        else
        {
            noIndentedJsonSerializer.Serialize(file, json);
        }
    }

    public string Serialize(JToken json) => JsonConvert.SerializeObject(json);

    public JToken Deserialize(string jsonString)
        => JsonConvert.DeserializeObject<JToken>(jsonString) is JToken jt
            ? jt
            : throw new ArgumentException($"can not parse Json String. {jsonString}");

    public (string Path, JToken Value)? FilterJson(JToken json, string jsonPath)
    {
        var withRoot = GetWithRootFunction(json);
        var je = json.SelectToken(jsonPath);

        if (je == null)
        {
            return null;
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

    public JToken? GetJSONOrDefault(object? value)
    {
        if (value is JToken token)
        {
            return token;
        }

        return null;
    }

    public bool TryGetJSONObject(JToken json, [MaybeNullWhen(false)] out JObject jsonObject)
    {
        if (json is JObject jo)
        {
            jsonObject = jo;
            return true;
        }

        jsonObject = default;
        return false;
    }

    public bool TryGetJSONArray(JToken json, [MaybeNullWhen(false)] out JArray jsonArray)
    {
        if (json is JArray ja)
        {
            jsonArray = ja;
            return true;
        }

        jsonArray = default;
        return false;
    }

    public string SerializeSchema(JSchema jsonSchema) => JsonConvert.SerializeObject(jsonSchema);

    public JSchema DeserializeSchema(string jsonString) => JSchema.Parse(jsonString);

    public JSchema GetSchema(string typeName) => GetJSchema(typeName);

    public JSchema LoadSchemaFile(string filePath)
    {
        using var file = new StreamReader(filePath);
        using var json = new JsonTextReader(file);

        return JSchema.Load(json);
    }

    public void SaveSchemaFile(JSchema jsonSchema, string filePath, bool pretty)
    {
        using var file = new StreamWriter(filePath);

        if (pretty)
        {
            indentedJsonSerializer.Serialize(file, jsonSchema);
        }
        else
        {
            noIndentedJsonSerializer.Serialize(file, jsonSchema);
        }
    }

    public IJSONBuilder<JToken> NewJSONBuilder() => new JSONObjectBuilder();
}
