using System.Diagnostics.CodeAnalysis;

namespace Exlibris.Core;

public interface IObjectRegistry
{
    IObjectHandle RegisterObject(object value, object? misc = null);

    bool TryGetObjectHandle(string key, [MaybeNullWhen(false)] out IObjectHandle handle);

    IObjectHandle? GetObjectHandle(string key);

    IObjectHandle GetObjectHandleOrThrow(string key);

    bool TryGetObject(string key, [MaybeNullWhen(false)] out object value);

    bool TryGetObject<T>(string key, [MaybeNullWhen(false)] out T value);

    object? GetObject(string key);

    object GetObjectOrThrow(string key);

    T GetObjectOrThrow<T>(string key);

    void RemoveObject(string key);

    int Count { get; }

    IEnumerable<(string Key, IObjectHandle Handle)> Handles { get; }
}
