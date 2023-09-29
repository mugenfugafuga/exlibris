using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.Linq;

namespace Exlibris.Core.JSONs.JSONNet
{
    internal class JSONObjectBuilder : IJSONBuilder<JToken>
    {
        private JToken root;
        private int indexer = 0;

        private static JToken CreateJToken(object value)
            => value == null ?
                JValue.CreateNull() :
                value is JToken jt ? jt.DeepClone() : new JValue(value);

        public JToken Build() => root?.DeepClone() ?? JValue.CreateNull();

        public IJSONBuilder<JToken> Append(string path, object value)
        {
            var keys = JSONUtil.SplitPath(JSONUtil.MakePathRight(path)).ToArray();

            if (keys.Length == 1)
            {
                UpdateRoot(path, keys[0], value);
                return this;
            }

            var secondLast = GetSecondLastToken(path, keys.Take(keys.Length - 1));

            var leafKey = keys.Last();

            if (leafKey.IsArrayItem)
            {
                if (secondLast[leafKey.Name] == null)
                {
                    secondLast[leafKey.Name] = new JArray();
                }

                if (secondLast[leafKey.Name] is JArray ja)
                {
                    if (ja.Count < leafKey.ArrayIndex + 1)
                    {
                        for (var i = ja.Count; i < leafKey.ArrayIndex; ++i)
                        {
                            ja.Add(JValue.CreateNull());
                        }
                        ja.Add(CreateJToken(value));
                        return this;
                    }
                    else if (ja[leafKey.ArrayIndex].Type == JTokenType.Null)
                    {
                        ja.Remove(ja[leafKey.ArrayIndex]);
                        ja[leafKey.ArrayIndex - 1].AddAfterSelf(CreateJToken(value));
                        return this;
                    }
                }
            }
            else
            {
                if (secondLast[leafKey.Name] == null)
                {
                    secondLast[leafKey.Name] = CreateJToken(value);
                    return this;
                }
            }

            throw new ArgumentException($"Node corresponding to {path} already exists. current json : {root}");
        }

        public IJSONBuilder<JToken> AddArrayElement(object value) => Append($"$[{indexer++}]", value);

        private JToken GetSecondLastToken(string path, IEnumerable<Key> keysWithoutLast)
            => keysWithoutLast.Aggregate((JToken)null, (prev, key) => PickUpCurrentToken(path, prev, key)) ?? throw new Exception($"unreachable");

        private JToken PickUpCurrentToken(string path, JToken prev, Key currentKey)
        {
            Debug.Assert((currentKey.IsRoot && prev == null) || (!currentKey.IsRoot && prev != null),
                "prev should be null, when currenKey is the root/first key ('$' or '$[number]' ) or vice versa.");

            if (currentKey.IsArrayItem)
            {
                JToken jt;

                if (prev == null)
                {
                    if (root == null)
                    {
                        root = new JArray();
                    }
                    jt = root;

                }
                else
                {
                    if (prev[currentKey.Name] == null)
                    {
                        prev[currentKey.Name] = new JArray();
                    }

                    jt = prev[currentKey.Name];
                }

                if (jt is JArray ja)
                {
                    return GetOrAddJObjectToJArray(path, ja, currentKey.ArrayIndex);
                }
                else
                {
                    throw new ArgumentException($"the node corresponding to {currentKey} should be jarray. current json : {root}, path : {path}");
                }
            }
            else
            {
                JToken jt;

                if (prev == null)
                {
                    if (root == null)
                    {
                        root = new JObject();
                    }
                    jt = root;
                }
                else
                {
                    if (prev[currentKey.Name] == null)
                    {
                        prev[currentKey.Name] = new JObject();
                    }
                    jt = prev[currentKey.Name];

                }

                if (jt is JObject jo)
                {
                    return jo;
                }
                else
                {
                    throw new ArgumentException($"the node corresponding to {currentKey} should be jobject. current json : {root}, path : {path}");
                }
            }
        }

        private JToken GetOrAddJObjectToJArray(string path, JArray array, int index)
        {
            if (array.Count <= index)
            {
                for (var i = array.Count; i < index; ++i)
                {
                    array.Add(JValue.CreateNull());
                }
                array.Add(new JObject());
            }

            if (array[index].Type == JTokenType.Null)
            {
                array.Remove(array[index]);
                array[index - 1].AddAfterSelf(new JObject());
            }

            return array[index] is JObject jo ? jo :
                throw new ArgumentException($"can not resolve the path. current json : {root}, path : {path}");

        }

        private void UpdateRoot(string path, Key rootKey, object value)
        {
            if ((root != null && !rootKey.IsArrayItem) || (root != null && rootKey.IsArrayItem && !(root is JArray)))
            {
                throw new ArgumentException($"The type of root node differs from the type expected from path. current json : {root}, path : {path}, value : {value?.ToString() ?? "null"}");
            }

            if (root == null && !rootKey.IsArrayItem)
            {
                root = CreateJToken(value);
                return;
            }

            if (root == null)
            {
                root = new JArray();
            }

            var ja = (JArray)root; // logically, root is a JArray.

            if (ja.Count <= rootKey.ArrayIndex)
            {
                for (var i = ja.Count; i < rootKey.ArrayIndex; ++i)
                {
                    ja.Add(JValue.CreateNull());
                }
                ja.Add(CreateJToken(value));
            }
            else if (ja[rootKey.ArrayIndex].Type == JTokenType.Null)
            {
                ja.Remove(ja[rootKey.ArrayIndex]);
                ja[rootKey.ArrayIndex - 1].AddAfterSelf(CreateJToken(value));
            }
            else
            {
                throw new ArgumentException($"can not append value. current json : {root}, path : {path}, value : {value?.ToString() ?? "null"}");
            }

        }
    }
}