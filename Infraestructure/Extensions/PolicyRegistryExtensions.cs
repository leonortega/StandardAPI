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
            var pollySettings = new PollySettings();
            configuration.GetSection("Polly").Bind(pollySettings);

            var registry = new PolicyRegistry();

            var retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(
                    pollySettings.RetryCount,
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    (exception, timeSpan, retryAttempt, context) =>
                    {
                        var logger = context.GetLogger();
                        logger?.LogWarning(exception, "Retry {RetryAttempt} after {TimeSpan} due to {ExceptionMessage}", retryAttempt, timeSpan, exception.Message);
                    });

            var circuitBreakerPolicy = Policy
                .Handle<Exception>()
                .CircuitBreakerAsync(
                    exceptionsAllowedBeforeBreaking: pollySettings.CircuitBreakerExceptionsAllowedBeforeBreaking,
                    durationOfBreak: pollySettings.CircuitBreakerDuration,
                    onBreak: (exception, duration, context) =>
                    {
                        var logger = context.GetLogger();
                        logger?.LogError(exception, "Circuit broken for {Duration} due to {ExceptionMessage}", duration, exception.Message);
                    },
                    onReset: context =>
                    {
                        var logger = context.GetLogger();
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
            return context.TryGetValue("Logger", out var logger) ? logger as ILogger : null;
        }
    }
}
