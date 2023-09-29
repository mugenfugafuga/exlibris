using System.Collections.Generic;

namespace Exlibris.Core
{
    public interface IObjectRegistry
    {
        IObjectHandle RegisterObject(object value, object misc = null);

        bool TryGetObjectHandle(string key, out IObjectHandle handle);

        IObjectHandle GetObjectHandle(string key);

        IObjectHandle GetObjectHandleOrThrow(string key);

        bool TryGetObject(string key, out object value);

        bool TryGetObject<T>(string key, out T value);

        object GetObject(string key);

        object GetObjectOrThrow(string key);

        T GetObjectOrThrow<T>(string key);

        void RemoveObject(string key);

        int Count { get; }

        IEnumerable<(string Key, IObjectHandle Handle)> Handles { get; }
    }
}