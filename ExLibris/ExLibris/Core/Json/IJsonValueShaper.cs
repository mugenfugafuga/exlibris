namespace ExLibris.Core.Json
{
    public interface IJsonValueShaper
    {
        bool ShouldShape(string key, object value);


        object Shape(object value);
    }
}
