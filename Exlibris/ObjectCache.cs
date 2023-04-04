using Exlibris.Core;
using Exlibris.Excel;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace Exlibris;
public class ObjectCache
{
    private readonly ConcurrentDictionary<Type, object> cacheOfCashes = new();
    private readonly ExlibrisLock locker = new();

    private ConcurrentDictionary<ExcelAddress, (DateTime CacheTime, T Value)> GetCache<T>()
        => (ConcurrentDictionary<ExcelAddress, (DateTime, T)>)
            cacheOfCashes.GetOrAdd(typeof(T), (_) => new ConcurrentDictionary<ExcelAddress, (DateTime, T)>());

    private void Remove<T>(ExcelAddress address, DateTime cachedTime)
    {
         var cache = GetCache<T>();

        if (cache.TryGetValue(address, out var v0) && v0.CacheTime == cachedTime)
        {
            using var locking = locker.GetWriteLock();

            if (cache.TryGetValue(address, out var v1) && v1.CacheTime == cachedTime)
            {
                cache.TryRemove(address, out _);
            }
        }
    }

    public bool TryGetValue<T>(ExcelAddress address, [MaybeNullWhen(false)] out T value)
    {
        var cache = GetCache<T>();

        using (var locking = locker.GetReadLock())
        {
            if (cache.TryGetValue(address, out var v))
            {
                value = v.Value;
                return true;
            }
        }

        value = default;
        return false;
    }

    public T GetOrThrow<T>(ExcelAddress address)
    {
        var cache = GetCache<T>();

        using (var locking = locker.GetReadLock())
        {
            if (cache.TryGetValue(address, out var v))
            {
                return v.Value;
            }
        }

        throw new InvalidOperationException($"{typeof(T)} for {address} is not cached.");
    }

        public IDisposable Cashe<T>(ExcelAddress address, T Value)
    {
        var cache = GetCache<T>();
        var cachedTime = DateTime.Now;

        using (var locking = locker.GetWriteLock())
        {
            cache.AddOrUpdate(
                address,
                (_) => (cachedTime, Value),
                (_0, _1) => (cachedTime, Value));
        }

        return new CacheHandle<T>(address, cachedTime, this);
    }

    private class CacheHandle<T> : AbstractDisposable
    {
        private readonly ExcelAddress address;
        private readonly DateTime cachedTime;
        private readonly ObjectCache objectCache;

        public CacheHandle(ExcelAddress address, DateTime cachedTime, ObjectCache objectCache)
        {
            this.address = address;
            this.cachedTime = cachedTime;
            this.objectCache = objectCache;
        }

        public override void OnDisposing() => objectCache.Remove<T>(address, cachedTime);
    }
}
