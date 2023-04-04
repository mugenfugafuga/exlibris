using System.Text.RegularExpressions;

namespace Exlibris.Core.JSONs;

public static class JSONUtil
{
    private const char delimiter = '.';
    private const char quote = '\'';
    private const char indexBra = '[';

    private const string rootKeyString = "$";
    private const string quoteString = "'";
    private const string arrayIndexPattern = @"\[(\d+)\]$";

    private static readonly string rootDelimString = $"{rootKeyString}{delimiter}";

    private static readonly Regex arrayIndexRegex = new(arrayIndexPattern);

    public static IEnumerable<Key> SplitPath(string path)
    {
        var start = 0;
        var inquote = false;

        for(var i = 0; i < path.Length; i++)
        {
            if (path[i] == quote)
            {
                inquote = !inquote;
            }
            else if (path[i] == delimiter && !inquote)
            {
                yield return NewKey(path[start..i], false);
                start = i + 1;
            }
        }

        yield return NewKey(path[start..], true);
    }

    private static Key NewKey(string key, bool leaf)
    {
        var k = key.Replace(quoteString, string.Empty);
        var arrayMatch = arrayIndexRegex.Match(k);

        var array = arrayMatch.Success;
        var index = array ?
                Convert.ToInt32(arrayMatch.Groups[1].Value) : -1;
        var name = array ? k[..k.IndexOf(indexBra)] : k;
        var root = name.Equals(rootKeyString);

        return new Key(root, array, name, index, !leaf);
    }

    public static string BuildPath(IEnumerable<Key> keys)
        => string.Join(delimiter, keys);

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

        if (path.StartsWith(indexBra))
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
