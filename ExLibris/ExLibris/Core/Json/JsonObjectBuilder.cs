using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExLibris.Core.Json
{
    public class JsonObjectBuilder
    {
        private Dictionary<string, object> shortcut = new Dictionary<string, object>();
        private object root = null;

        public JsonObjectBuilder()
        {
        }

        public JsonObjectBuilder AddJsonValue(string keyPath, object value)
        {
            if (JsonUtility.IsRootElement(keyPath))
            {

                return SetOnlyRootValue(value);
            }

            var je = PickUpPreviousElement(keyPath);

            var (_, lastkey) = JsonUtility.SplitLastKey(keyPath);

            if (JsonUtility.IsJsonArray(lastkey))
            {
                var ja = JsonUtility.CastJsonArray(je);
                var index = JsonUtility.GetJsonArrayIndex(lastkey);
                ResizeJsonArray(ja, index);
                ja[index] = value;
            }
            else
            {
                var jd = JsonUtility.CastJsonDictionary(je);
                jd[lastkey] = value;

            }

            return this;
        }

        public JsonObjectBuilder SetOnlyRootValue(object rootValue)
        {
            if (root != null)
            {
                throw new Exception($"root exists already. value {rootValue}");
            }

            root = rootValue;

            return this;
        }

        public JsonObject BuildJsonObject()
            => new JsonObject
            {
                Object = root,
            };

        public dynamic BuildDynamic() => root;

        private object PickUpPreviousElement(string keyPath)
        {
            var (frontPart, _) = JsonUtility.SplitLastKey(keyPath);

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

            for(var i = 0; i < keys.Length - 1; ++i)
            {
                run = DescendJsonElement(run, keys[i], keys[i + 1]);
            }

            shortcut[frontPart] = run;

            return run;
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
            for(var i = jarry.Count; i <= index + 1; ++i)
            {
                jarry.Add(null);
            }
        }

    }
}
