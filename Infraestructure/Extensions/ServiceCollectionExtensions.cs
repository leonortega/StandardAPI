using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
            services.AddScoped<IProductRepository, ProductRepository>();

            var dbConnectionString = configuration.GetConnectionString("DefaultConnection");
            services.AddSingleton(new DatabaseConnectionFactory(dbConnectionString!));

            var redisSettings = new RedisSettings();
            configuration.GetSection("Redis").Bind(redisSettings);

            var redisConnection = ConnectionMultiplexer.Connect(redisSettings.ConnectionString!);
            services.AddSingleton<IConnectionMultiplexer>(sp => redisConnection);
            services.AddSingleton(sp => new RedisCacheService(redisConnection, redisSettings.DefaultCacheExpiryMinutes));

            services.AddSingleton(serviceProvider =>
            {
                var logger = serviceProvider.GetRequiredService<ILogger<PollyPolicyBuilder>>();
                var pollySettings = new PollySettings();
                configuration.GetSection("Polly").Bind(pollySettings);

                return new
                {
                    RetryPolicy = PollyPolicyBuilder.GetRetryPolicy(pollySettings.RetryCount, logger),
                    CircuitBreakerPolicy = PollyPolicyBuilder.GetCircuitBreakerPolicy(
                        pollySettings.CircuitBreakerExceptionsAllowedBeforeBreaking,
                        pollySettings.CircuitBreakerDuration,
                        logger)
                };
            });

            return services;
        }
    }
}
