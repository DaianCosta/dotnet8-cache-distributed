using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace api.Services;

public sealed class DistributedCacheService : IDistributedCacheService
{
    private readonly ILogger<DistributedCacheService> _logger;
    private readonly IDistributedCache _distributedCache;
    private const string PREFIX = "my-api-xpto_";

    public DistributedCacheService(ILogger<DistributedCacheService> logger,
    IDistributedCache distributedCache)
    {
        _logger = logger;
        _distributedCache = distributedCache;
    }

    public async Task<string?> GetAsync(string key)
    {

        var jsonValue = await GetCacheAsync(GenerateKey(key));

        if (!string.IsNullOrWhiteSpace(jsonValue))
        {
            var result = JsonSerializer.Deserialize<string>(jsonValue);
            return result == null ? throw new InvalidOperationException() : result;
        }

        return null;
    }

    public async Task<T?> GetAsync<T>(string key)
    {

        var jsonValue = await GetCacheAsync(GenerateKey(key));

        if (!string.IsNullOrWhiteSpace(jsonValue))
        {
            var result = JsonSerializer.Deserialize<T>(jsonValue);
            return result == null ? throw new InvalidOperationException() : result;
        }

        return default(T);
    }

    public async Task<string> GetOrCreateAsync(
            string key,
            string value, TimeSpan expiresIn)
    {
        var jsonValue = await GetCacheAsync(GenerateKey(key));

        if (!string.IsNullOrWhiteSpace(jsonValue))
        {
            var result = JsonSerializer.Deserialize<string>(jsonValue);
            return result == null ? throw new InvalidOperationException() : result;
        }

        var options = CreateDistributedCacheEntryOptions(expiresIn);

        await SetCacheAsync(GenerateKey(key), value, options);

        return value;
    }

    public async Task<T> GetOrCreateAsync<T>(
            string key,
            T value, TimeSpan expiresIn)
    {
        var jsonValue = await GetCacheAsync(GenerateKey(key));

        if (!string.IsNullOrWhiteSpace(jsonValue))
        {
            var result = JsonSerializer.Deserialize<T>(jsonValue);
            return result == null ? throw new InvalidOperationException() : result;
        }

        var options = CreateDistributedCacheEntryOptions(expiresIn);

        await SetCacheAsync(GenerateKey(key), value, options);

        return value;
    }


    private static DistributedCacheEntryOptions CreateDistributedCacheEntryOptions(TimeSpan expiredIn)
    {
        return new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiredIn
        };
    }


    private async Task SetCacheAsync<T>(string key, T value, DistributedCacheEntryOptions options)
    {
        if (value == null)
        {
            return;
        }

        try
        {
            var jsonValue = JsonSerializer.Serialize(value);
            await _distributedCache.SetStringAsync(GenerateKey(key), jsonValue, options);
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Erro ao realizar o set no cache pela chave {CacheKey} {Exception}", key, ex);
        }
    }

    private async Task<string?> GetCacheAsync(string key)
    {
        var jsonValue = string.Empty;

        try
        {
            jsonValue = await _distributedCache
                .GetStringAsync(GenerateKey(key));
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Erro ao realizar o get no cache pela chave {CacheKey} {Exception}", key, ex);
        }

        return jsonValue;
    }

    private string GenerateKey(string key)
    {
        return $"{PREFIX}_{key}";
    }

}
