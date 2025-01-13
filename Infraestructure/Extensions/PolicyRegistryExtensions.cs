using Polly;
using Polly.Registry;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StandardAPI.Infraestructure.Services;
using StandardAPI.Infraestructure.Settings;

namespace StandardAPI.Infraestructure.Extensions
{
    public static class PolicyRegistryExtensions
    {
        public static IServiceCollection AddPollyPolicies(this IServiceCollection services, IConfiguration configuration)
        {
            ArgumentNullException.ThrowIfNull(configuration);

            var pollySettings = new PollySettings();
            configuration.GetSection("Polly").Bind(pollySettings);

            var registry = new PolicyRegistry();

            Polly.Retry.AsyncRetryPolicy retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(
                    pollySettings.RetryCount,
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    static (exception, timeSpan, retryAttempt, context) =>
                    {
                        ILogger? logger = context.GetLogger();
                        logger?.LogWarning(exception, "Retry {RetryAttempt} after {TimeSpan} due to {ExceptionMessage}", retryAttempt, timeSpan, exception.Message);
                    });

            Polly.CircuitBreaker.AsyncCircuitBreakerPolicy circuitBreakerPolicy = Policy
                .Handle<Exception>()
                .CircuitBreakerAsync(
                    exceptionsAllowedBeforeBreaking: pollySettings.CircuitBreakerExceptionsAllowedBeforeBreaking,
                    durationOfBreak: pollySettings.CircuitBreakerDuration,
                    onBreak: (exception, duration, context) =>
                    {
                        ILogger? logger = context.GetLogger();
                        logger?.LogError(exception, "Circuit broken for {Duration} due to {ExceptionMessage}", duration, exception.Message);
                    },
                    onReset: context =>
                    {
                        ILogger? logger = context.GetLogger();
                        logger?.LogInformation("Circuit breaker reset.");
                    },
                    onHalfOpen: () =>
                    {
                        // Optional half-open logic
                    });

            registry.Add("RetryAndCircuitBreaker", retryPolicy.WrapAsync(circuitBreakerPolicy));

            services.AddSingleton<IPolicyRegistry<string>>(registry);
            services.AddSingleton<ResilientPolicyExecutor>();

            return services;
        }
    }

    public static class PollyContextExtensions
    {
        public static ILogger? GetLogger(this Context context)
        {
            ArgumentNullException.ThrowIfNull(context);

            return context.TryGetValue("Logger", out object? logger) ? logger as ILogger : null;
        }
    }
}
