namespace HopFrame.Security.Authentication.OpenID;

public interface ICacheProvider {
    Task<TItem> GetOrCreate<TItem>(string key,  Func<Task<TItem>> factory) where TItem : class;
    Task Set<TItem>(string key, TItem value, TimeSpan ttl);
}