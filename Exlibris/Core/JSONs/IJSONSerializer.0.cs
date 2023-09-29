using System;
using System.Collections.Generic;
using System.Xml;

namespace Exlibris.Core.JSONs
{
    public interface IJSONSerializer
        <
            JSONObjectType,
            JSONArrayType,
            JSONValueType,
            JSONBaseType,
            JSONSchema
        >
        where JSONObjectType : JSONBaseType
        where JSONArrayType : JSONBaseType
        where JSONValueType : JSONBaseType
    {
        object ToObject(JSONBaseType json, Type objectType);

        T ToObject<T>(JSONBaseType json);

        JSONBaseType FromObject(object obj);

        JSONBaseType LoadFile(string filePath);

        void SaveFile(JSONBaseType json, string filePath, bool pretty);

        string Serialize(JSONBaseType json);

        JSONBaseType Deserialize(string jsonString);

        (string Path, JSONBaseType Value) FilterJson(JSONBaseType json, string jsonPath);

        IEnumerable<(string Path, JSONBaseType Value)> FilterJsons(JSONBaseType json, string jsonPath);

        IEnumerable<(string Path, JSONValueType Value)> GetValues(JSONBaseType json);

        bool IsJSONObject(JSONBaseType json);

        bool IsJSONArray(JSONBaseType json);

        JSONBaseType GetJSONOrDefault(object value);

        bool TryGetJSONObject(JSONBaseType json, out JSONObjectType jsonObject);

        bool TryGetJSONArray(JSONBaseType json, out JSONArrayType jsonArray);

        string SerializeSchema(JSONSchema jsonSchema);

        JSONSchema DeserializeSchema(string jsonString);

        JSONSchema GetSchema(string typeName);

        JSONSchema LoadSchemaFile(string filePath);

        void SaveSchemaFile(JSONSchema jsonSchema, string filePath, bool pretty);

        XmlDocument ToXml(JSONBaseType json);

        JSONBaseType FromXml(XmlDocument xml);

        IJSONBuilder<JSONBaseType> NewJSONBuilder();
    }
}