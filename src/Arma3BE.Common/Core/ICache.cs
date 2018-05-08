using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;

namespace Arma3BEClient.Common.Core
{
    public interface ICache<T>
    {
        void Add(string key, T value);
        void Remove(string key);
        T Get(string key);
        T[] GetAll();
        T[] GetAll(IEnumerable<string> keys);
    }

    public interface ICacheFactory
    {
        ICache<T> CreateCache<T>(TimeSpan expireTIme);
    }

    internal class Cache<T> : ICache<T>
    {
        readonly MemoryCache _cache = new MemoryCache(Guid.NewGuid().ToString());
        readonly TimeSpan _expireTime;

        public Cache(TimeSpan expireTime)
        {
            _expireTime = expireTime;
        }

        public void Add(string key, T value)
        {
            var cacheItem = new CacheItem(key, value);
            // Ваши политики протухания кеша
            //var policy = new CacheItemPolicy() { SlidingExpiration = _expireTime };
            var policy = new CacheItemPolicy { SlidingExpiration = _expireTime };
            _cache.Add(cacheItem, policy);
        }

        public void Remove(string key)
        {
            _cache.Remove(key);
        }

        public T Get(string key)
        {
            var item = _cache.GetCacheItem(key);
            var ii = _cache[key];
            return (T)_cache.Get(key);
        }

        public T[] GetAll()
        {
            return _cache.Select(x => x.Value).OfType<T>().ToArray();
        }

        public T[] GetAll(IEnumerable<string> keys)
        {
            return _cache.GetValues(keys)?.OfType<T>().ToArray() ?? new T[0];
        }
    }

    public class CacheFactory : ICacheFactory
    {
        public ICache<T> CreateCache<T>(TimeSpan expireTIme)
        {
            return new Cache<T>(expireTIme);
        }
    }
}