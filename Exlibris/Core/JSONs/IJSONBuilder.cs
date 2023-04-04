namespace Exlibris.Core.JSONs;
public interface IJSONBuilder<JSONBaseObjectType>
{
    IJSONBuilder<JSONBaseObjectType> Append(string path, object? value);
    IJSONBuilder<JSONBaseObjectType> AddArrayElement(object? value);
    JSONBaseObjectType Build();
}
