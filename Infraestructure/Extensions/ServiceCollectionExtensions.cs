using Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using StandardAPI.Domain.Interfaces;
using StandardAPI.Infraestructure.Persistence;
using StandardAPI.Infraestructure.Services;
using StandardAPI.Infraestructure.Settings;

namespace StandardAPI.Infraestructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration, ILogger logger)
        {
            services.AddScoped<IProductRepository, ProductRepository>();

            var dbConnectionString = configuration.GetConnectionString("DefaultConnection");
            services.AddSingleton(new DatabaseConnectionFactory(dbConnectionString!));

            var redisConfig = configuration.GetSection("Redis");
            var redisConnectionString = redisConfig["ConnectionString"];
            var defaultExpiryString = redisConfig["DefaultCacheExpiryMinutes"];
            var defaultExpiry = defaultExpiryString != null ? int.Parse(defaultExpiryString) : 30;

            var redisConnection = ConnectionMultiplexer.Connect(redisConnectionString!);
            services.AddSingleton<IConnectionMultiplexer>(sp => redisConnection);
            services.AddSingleton(sp => new RedisCacheService(redisConnection, defaultExpiry));

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
