using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace api.Services;

public class DistributedCacheServiceTest
{
    private readonly IDistributedCacheService _distributedCacheService;
    private readonly ILogger<DistributedCacheService> _logger;
    private readonly IDistributedCache _distributedCache;
    public DistributedCacheServiceTest()
    {
        _logger = Substitute.For<ILogger<DistributedCacheService>>();
        _distributedCache = Substitute.For<IDistributedCache>();
        _distributedCacheService = new DistributedCacheService(_logger, _distributedCache);
    }

    [Fact]
    public async Task GetCache_SingleText_ReturnsValueString()
    {
        var key = "my-api-xpto_my-person";
            var value = "014.838.290-81";
            var jsonValueSerialize = JsonSerializer.Serialize(value);
            var jsonValueBytes = JsonSerializer.SerializeToUtf8Bytes(jsonValueSerialize);

        _distributedCache.GetAsync(Arg.Any<string>())
        .Returns(jsonValueBytes);

        var result = await _distributedCacheService.GetCacheAsync<string>(key);

        Assert.Equal(value, result);
    }

    [Fact]
    public async Task AddCache_Generic_ReturnsValueGeneric()
    {
        var key = "my-api-xpto_my-person";
        var value = new PersonBuilder() { FirstName = "Daian", LastName = "Costa" };

        _distributedCache.GetAsync(Arg.Any<string>())
        .ReturnsNull();

        var result = await _distributedCacheService.GetOrCreateCacheAsync<PersonBuilder>(key, value, TimeSpan.FromMinutes(1));

        await _distributedCache.Received(1).SetAsync(
            Arg.Any<string>(),
            Arg.Any<byte[]>(),
        Arg.Any<DistributedCacheEntryOptions>()
        );
        
        Assert.Equal(value, result);
    }
}

public class PersonBuilder()
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
}