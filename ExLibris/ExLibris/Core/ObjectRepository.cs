using System.Collections.Concurrent;

namespace ExLibris.Core
{
    public class ObjectRepository
    {
        private readonly ConcurrentDictionary<string, object> cache = new ConcurrentDictionary<string, object>();

        public void RegisterObject(string key, object obj) => cache.AddOrUpdate(key, obj, (_1, _2) => obj);

        public object GetObject(string key) => cache[key];

        public T GetObject<T>(string key) => (T)GetObject(key);

        public bool TryGetObject(string key, out object obj) => cache.TryGetValue(key, out obj);

        public bool TryGetObject<T>(string key, out T obj)
        {
            if(TryGetObject(key, out object o))
            {
                obj = (T)o;
                return true;
            }

            obj = default(T);
            return false;
        }

        public void Remove(string key) => cache.TryRemove(key, out var _);
    }
}
