using api.Services;

namespace api;

public static class RegisterCacheExtensions
{
    public static IServiceCollection AddCacheFramework(
           this IServiceCollection services)
    {
        //bool.TryParse(Environment.GetEnvironmentVariable("IS_REDIS"), out bool isRedis);
        bool isRedis = false;
        if (isRedis)
        {
            services.AddStackExchangeRedisCache(options =>
                {
                    options.Configuration = "localhost:6379";
                });
            services.AddStackExchangeRedisOutputCache(
            options =>
            {
                options.Configuration = "localhost:6379";
            });
        }
        else
        {
            services.AddDistributedMemoryCache();
            services.AddOutputCache();
        }

        services.AddScoped<IDistributedCacheService, DistributedCacheService>();

        return services;
    }

}
