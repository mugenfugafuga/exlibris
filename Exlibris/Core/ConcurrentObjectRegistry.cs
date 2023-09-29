using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Exlibris.Core
{
    public class ConcurrentObjectRegistry : IObjectRegistry
    {
        private readonly ConcurrentDictionary<string, ObjectHandle> cache = new ConcurrentDictionary<string, ObjectHandle>();

        public IObjectHandle RegisterObject(object value, object misc = null)
        {
            var key = $"{value.GetType()}:{Guid.NewGuid().ToString().ToUpper()}";
            var handle = new ObjectHandle(cache, key, value, misc);

            cache[key] = handle;

            return handle;
        }

        public bool TryGetObjectHandle(string key, out IObjectHandle handle)
        {
            if (cache.TryGetValue(key, out var objectHandle))
            {
                handle = objectHandle;
                return true;
            }

            handle = null;
            return false;
        }

        public IObjectHandle GetObjectHandle(string key)
            => cache.TryGetValue(key, out var objectHandle) ?
                objectHandle : null;

        public IObjectHandle GetObjectHandleOrThrow(string key)
            => cache.TryGetValue(key, out var objectHandle) ?
            objectHandle :
            throw new KeyNotFoundException($"key does not exist. key : {key}");

        public bool TryGetObject(string key, out object value)
        {
            if (TryGetObjectHandle(key, out var handle))
            {
                value = handle.Value;
                return true;
            }
            else
            {
                value = null;
                return false;
            }
        }

        public bool TryGetObject<T>(string key, out T value)
        {
            if (TryGetObjectHandle(key, out var handle))
            {
                if (handle.Value is T v)
                {
                    value = v;
                    return true;
                }
            }

            value = default;
            return false;
        }


        public object GetObject(string key)
            => GetObjectHandle(key)?.Value;

        public object GetObjectOrThrow(string key)
        => GetObjectHandleOrThrow(key).Value;

        public T GetObjectOrThrow<T>(string key)
            => GetObjectHandleOrThrow(key).ValueOrThrow<T>();

        public void RemoveObject(string key) => GetObjectHandleOrThrow(key).Dispose();

        public int Count => cache.Count;

        public IEnumerable<(string Key, IObjectHandle Handle)> Handles => cache.Select(k => (k.Key, (IObjectHandle)k.Value));

        private class ObjectHandle : AbstractDisposable, IObjectHandle
        {
            public string Key { get; }

            public object Value { get; }

            public object Misc { get; }

            private readonly ConcurrentDictionary<string, ObjectHandle> cache;

            public ObjectHandle
            (
                ConcurrentDictionary<string, ObjectHandle> cache,
                string key,
                object value,
                object misc
            )
            {
                this.cache = cache;
                Key = key;
                Value = value;
                Misc = misc;
            }

            public T GetValue<T>() => Value is T val ? val : default;

            public T ValueOrThrow<T>()
                => Value is T val ?
                val : throw new InvalidCastException($"cannot convert to {typeof(T)}. type of Value : {Value.GetType()}");

            public override void OnDisposing()
            {
                this.cache.TryRemove(Key, out var _);

                if (Value is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }

        }
    }
}