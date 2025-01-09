using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly.CircuitBreaker;
using Polly;
using StackExchange.Redis;
using StandardAPI.Domain.Interfaces;
using StandardAPI.Infraestructure.Persistence;
using StandardAPI.Infraestructure.Repositories;
using StandardAPI.Infraestructure.Services;
using StandardAPI.Infraestructure.Settings;

namespace StandardAPI.Infraestructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            var dbConnectionString = configuration.GetConnectionString("DefaultConnection");
            services.AddSingleton(new DatabaseConnectionFactory(dbConnectionString!));

            var redisSettings = new RedisSettings();
            configuration.GetSection("Redis").Bind(redisSettings);

            var redisConnection = ConnectionMultiplexer.Connect(redisSettings.ConnectionString!);
            services.AddSingleton<IConnectionMultiplexer>(sp => redisConnection);
            services.AddSingleton(sp => new RedisCacheService(redisConnection, redisSettings.DefaultCacheExpiryMinutes));

            services.AddPollyPolicies(configuration);

            services.AddScoped<IProductRepository, ProductRepository>();

            return services;
        }
    }
}
