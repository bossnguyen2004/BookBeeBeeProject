using Microsoft.Extensions.Caching.Memory;

namespace BookBee.Services.CacheService
{
    public class InMemoryCacheService : ICacheService
    {
        private readonly IMemoryCache _memoryCache;

        public InMemoryCacheService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }


        public T Get<T>(string key)
        {
            return _memoryCache.TryGetValue(key, out T value) ? value : default;
        }


        public void Set<T>(string key, T value, TimeSpan expiration)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration
            };
            _memoryCache.Set(key, value, cacheEntryOptions);
        }


        public void Remove(string key)
        {
            _memoryCache.Remove(key);
        }


        public bool Exists(string key)
        {
            return _memoryCache.TryGetValue(key, out _);
        }
    }
}
