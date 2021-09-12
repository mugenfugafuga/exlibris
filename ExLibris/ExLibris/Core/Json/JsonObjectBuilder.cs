using System;
using System.Collections.Generic;
using System.Linq;

namespace ExLibris.Core.Json
{
    public class JsonObjectBuilder
    {
        private Dictionary<string, object> shortcut = new Dictionary<string, object>();
        private object root = null;
        private readonly ObjectRepository objectRepository;
        private readonly JsonValueConverter jsonValueConverter;

        public JsonObjectBuilder(ObjectRepository objectRepository, JsonValueConverter jsonValueConverter)
        {
            this.objectRepository = objectRepository;
            this.jsonValueConverter = jsonValueConverter;
        }

        public JsonObjectBuilder AddJsonValue(string keyPath, object value)
        {
            if (JsonUtility.IsRootElement(keyPath))
            {
                return SetOnlyRootValue(value);
            }

            var je = PickUpPreviousElement(keyPath);

            var (_, lastkey) = JsonUtility.SplitLastKey(keyPath);
            var v = RevealValue(value);

            if (JsonUtility.IsJsonArray(lastkey))
            {
                var ja = JsonUtility.CastJsonArray(je);
                var index = JsonUtility.GetJsonArrayIndex(lastkey);
                ResizeJsonArray(ja, index);
                ja[index] = v;
            }
            else
            {
                var jd = JsonUtility.CastJsonDictionary(je);
                jd[lastkey] = jsonValueConverter.Convert(lastkey, v);

            }

            return this;
        }

        public JsonObjectBuilder SetOnlyRootValue(object rootValue)
        {
            if (root != null)
            {
                throw new Exception($"root exists already. value {rootValue}");
            }

            root = RevealValue(rootValue);

            return this;
        }

        public object BuildJsonObject() => root;

        public dynamic BuildDynamic() => root;

        private object PickUpPreviousElement(string keyPath)
        {
            var (frontPart, _) = JsonUtility.SplitLastKey(keyPath);

            if (JsonUtility.IsRootElement(frontPart))
            {
                if (root == null)
                {
                    root = JsonUtility.NewJsonElement(keyPath);
                }

                return root;
            }
            else
            {
                if (shortcut.TryGetValue(frontPart, out var fpo))
                {
                    return fpo;
                }

                var keys = JsonUtility.SplitKeyPath(keyPath);

                if (root == null)
                {
                    root = JsonUtility.NewJsonElement(keys.First());
                }

                var run = root;

                for (var i = 0; i < keys.Length - 1; ++i)
                {
                    run = DescendJsonElement(run, keys[i], keys[i + 1]);
                }

                shortcut[frontPart] = run;

                return run;
            }
        }

        private object DescendJsonElement(object prev, string prevKey, string key)
        {
            if (JsonUtility.IsJsonArray(prevKey))
            {
                var jarray = JsonUtility.CastJsonArray(prev);
                var pindex = JsonUtility.GetJsonArrayIndex(prevKey);
                ResizeJsonArray(jarray, pindex);

                if(jarray[pindex] == null)
                {
                    jarray[pindex] = JsonUtility.NewJsonElement(key);
                }

                return jarray[pindex];
            }
            else
            {
                var jdict = JsonUtility.CastJsonDictionary(prev);
                if (jdict.TryGetValue(prevKey, out var jd))
                {
                    return jd;
                }
                else
                {
                    jdict[prevKey] = JsonUtility.NewJsonElement(key);
                    return jdict[prevKey];
                }
            }
        }

        private static void ResizeJsonArray(List<object> jarry, int index)
        {
            for(var i = jarry.Count; i <= index; ++i)
            {
                jarry.Add(null);
            }
        }

        private object RevealValue(object value)
        {
            if(value is string && objectRepository.TryGetObject((string)value, out var v))
            {
                return v;
            }

            return value;
        }
    }
}
