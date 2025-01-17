using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using StackExchange.Redis;
using StandardAPI.Domain.Entities;
using StandardAPI.Domain.Interfaces;
using StandardAPI.Infraestructure.Repositories;
using StandardAPI.Common.Settings;

namespace StandardAPI.Infraestructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddRedisCache(configuration);
            services.AddNpgsql(configuration);
            services.AddPollyPolicies(configuration);

            //Repositories
            services.AddRepository<Product>();
            services.AddScoped<IProductRepository, ProductRepository>();

            return services;
        }

        private static IServiceCollection AddRepository<TEntity>(this IServiceCollection services)
            where TEntity : class
        {
            services.AddScoped<IBaseRepository<TEntity>, BaseRepository<TEntity>>();

            return services;
        }

        public static IServiceCollection AddRedisCache(this IServiceCollection services, IConfiguration configuration)
        {
            ArgumentNullException.ThrowIfNull(configuration);

            var redisSettings = new RedisSettings();
            configuration.GetSection("Redis").Bind(redisSettings);

            services.AddStackExchangeRedisCache(options =>
            {
                options.ConfigurationOptions = ConfigurationOptions.Parse(redisSettings.ConnectionString!);
            });

            return services;
        }

        public static IServiceCollection AddNpgsql(this IServiceCollection services, IConfiguration configuration)
        {
            ArgumentNullException.ThrowIfNull(configuration);

            services.AddSingleton<string>(configuration.GetConnectionString("DefaultConnection")!);
            return services;
        }

        public static IServiceCollection AddPollyPolicies(this IServiceCollection services, IConfiguration configuration)
        {
            ArgumentNullException.ThrowIfNull(configuration);

            var pollySettings = new PollySettings();
            configuration.GetSection("Polly").Bind(pollySettings);

            services.AddTransient<AsyncRetryPolicy>(sp =>
                Policy
                    .Handle<Exception>()
                    .WaitAndRetryAsync(
                        retryCount: pollySettings.RetryCount,
                        sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(retryAttempt * pollySettings.RetryIntervalInSeconds)));

            services.AddTransient<AsyncCircuitBreakerPolicy>(sp =>
                Policy
                    .Handle<Exception>()
                    .CircuitBreakerAsync(
                        exceptionsAllowedBeforeBreaking: pollySettings.CircuitBreakerExceptionsAllowedBeforeBreaking,
                        durationOfBreak: TimeSpan.FromSeconds(pollySettings.CircuitBreakerDurationInSeconds)));

            return services;
        }
    }
}