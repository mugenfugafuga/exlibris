using System.Collections.Generic;
using System;
using System.Text.RegularExpressions;
using System.CodeDom;

namespace Exlibris.Core.JSONs
{
    public static class JSONUtil
    {
        private const char delimiter = '.';
        private const char quote = '\'';
        private const char indexBra = '[';

        private const string delimiterStr = ".";
        private const string indexBraStr = "[";

        private const string rootKeyString = "$";
        private const string quoteString = "'";
        private const string arrayIndexPattern = @"\[(\d+)\]$";

        private static readonly string rootDelimString = $"{rootKeyString}{delimiter}";

        private static readonly Regex arrayIndexRegex = new Regex(arrayIndexPattern);

        public static IEnumerable<Key> SplitPath(string path)
        {
            var start = 0;
            var inquote = false;

            for (var i = 0; i < path.Length; i++)
            {
                if (path[i] == quote)
                {
                    inquote = !inquote;
                }
                else if (path[i] == delimiter && !inquote)
                {
                    yield return NewKey(path.Substring(start, i - start), false);
                    start = i + 1;
                }
            }

            yield return NewKey(path.Substring(start), true);
        }

        private static Key NewKey(string key, bool leaf)
        {
            var k = key.Replace(quoteString, string.Empty);
            var arrayMatch = arrayIndexRegex.Match(k);

            var array = arrayMatch.Success;
            var index = array ?
                    Convert.ToInt32(arrayMatch.Groups[1].Value) : -1;
            var name = array ? k.Substring(0, k.IndexOf(indexBra)) : k;
            var root = name.Equals(rootKeyString);

            return new Key(root, array, name, index, !leaf);
        }

        public static string BuildPath(IEnumerable<Key> keys)
            => string.Join(delimiterStr, keys);

        public static string MakePathRight(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return rootKeyString;
            }

            if (path.StartsWith(rootKeyString))
            {
                return path;
            }

            if (path.StartsWith(indexBraStr))
            {
                return $"{rootKeyString}{path}";
            }
            else
            {
                return $"{rootKeyString}.{path}";
            }
        }

        public static string PathWithoutRoot(string path)
        {
            if (path.StartsWith(rootDelimString))
            {
                return path.Substring(rootDelimString.Length);
            }

            if (path.StartsWith(rootKeyString))
            {
                return path.Substring(rootKeyString.Length);
            }

            return path;
        }
    }
}