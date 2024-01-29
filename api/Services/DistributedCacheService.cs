using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace api.Services;

public sealed class DistributedCacheService : IDistributedCacheService
{
    private readonly ILogger<DistributedCacheService> _logger;
    private readonly IDistributedCache _distributedCache;
    private const string PREFIX = "my-api-xpto";

    public DistributedCacheService(ILogger<DistributedCacheService> logger,
    IDistributedCache distributedCache)
    {
        _logger = logger;
        _distributedCache = distributedCache;
    }


    public async Task<T?> GetCacheAsync<T>(string key)
    {

        var jsonValue = default(T);

        try
        {
            var getData = await _distributedCache.GetAsync(GenerateKey(key));
            if (getData != null)
            {
                var serializedGetData = JsonSerializer.Deserialize<string>(getData);
                if (!string.IsNullOrWhiteSpace(serializedGetData))
                {
                    jsonValue = JsonSerializer.Deserialize<T>(serializedGetData);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Error when performing cache get by key {CacheKey} {Exception} for object T", key, ex);
        }

        return jsonValue;
    }

    public async Task<T> GetOrCreateCacheAsync<T>(
            string key,
            T value, TimeSpan expiresIn)
    {
        try
        {
            var jsonValue = await _distributedCache.GetAsync(GenerateKey(key));

            if (jsonValue != null)
            {
                var result = JsonSerializer.Deserialize<T>(jsonValue);

                return result == null ? throw new InvalidOperationException() : result;
            }

            var options = CreateDistributedCacheEntryOptions(expiresIn);


            var jsonValueSerialize = JsonSerializer.Serialize(value);
            var jsonValueBytes = JsonSerializer.SerializeToUtf8Bytes(jsonValueSerialize);
            await _distributedCache.SetAsync(GenerateKey(key), jsonValueBytes, options);

        }
        catch (Exception ex)
        {
            _logger.LogWarning("Error when performing the set in the cache by key {CacheKey} {Exception}", key, ex);
        }

        return value;
    }


    private static DistributedCacheEntryOptions CreateDistributedCacheEntryOptions(TimeSpan expiredIn)
    {
        return new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiredIn
        };
    }

    private string GenerateKey(string key)
    {
        return $"{PREFIX}:{key}";
    }

}
