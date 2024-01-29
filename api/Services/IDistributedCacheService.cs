namespace api.Services;

public interface IDistributedCacheService
{
    Task<T?> GetAsync<T>(string key);
    Task<string?> GetAsync(string key);
    Task<T> GetOrCreateAsync<T>(string key, T value, TimeSpan expiresIn);
    Task<string> GetOrCreateAsync(string key, string value, TimeSpan expiresIn);

}
