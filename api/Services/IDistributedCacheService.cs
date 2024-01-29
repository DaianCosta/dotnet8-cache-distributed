namespace api.Services;

public interface IDistributedCacheService
{
    Task<T?> GetCacheAsync<T>(string key);
    Task<T> GetOrCreateCacheAsync<T>(string key, T value, TimeSpan expiresIn);

}
