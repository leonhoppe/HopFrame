using Microsoft.Extensions.Caching.Memory;

namespace HopFrame.Security.Authentication.OpenID.Implementation;

public class MemoryCacheProvider(IMemoryCache cache) : ICacheProvider {
    public Task<TItem> GetOrCreate<TItem>(string key, Func<Task<TItem>> factory) where TItem : class {
        if (cache.TryGetValue(key, out var value)) {
            return Task.FromResult(value as TItem);
        }

        return factory.Invoke();
    }

    public Task Set<TItem>(string key, TItem value, TimeSpan ttl) {
        cache.Set(key, value, ttl);
        return Task.CompletedTask;
    }
}