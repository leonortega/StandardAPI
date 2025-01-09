using Polly;
using Polly.Retry;
using Polly.CircuitBreaker;
using Microsoft.Extensions.Logging;

namespace StandardAPI.Infraestructure.Services
{
    public class PollyPolicyBuilder
    {
        public static AsyncRetryPolicy GetRetryPolicy(int retryCount, ILogger logger)
        {
            return Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(
                    retryCount,
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    (exception, timeSpan, retryCount, context) =>
                    {
                        logger.LogWarning(
                            exception,
                            "Retry {RetryCount} after {Delay} due to {ExceptionMessage}",
                            retryCount,
                            timeSpan,
                            exception.Message);
                    });
        }

        public static AsyncCircuitBreakerPolicy GetCircuitBreakerPolicy(
            int exceptionsAllowedBeforeBreaking,
            TimeSpan breakDuration,
            ILogger logger)
        {
            return Policy
                .Handle<Exception>()
                .CircuitBreakerAsync(
                    exceptionsAllowedBeforeBreaking: exceptionsAllowedBeforeBreaking,
                    durationOfBreak: breakDuration,
                    onBreak: (exception, duration) =>
                    {
                        logger.LogError(
                            exception,
                            "Circuit breaker triggered after {ExceptionsAllowedBeforeBreaking} exceptions. Circuit will remain open for {Duration}. Reason: {ExceptionMessage}",
                            exceptionsAllowedBeforeBreaking,
                            duration,
                            exception.Message);
                    },
                    onReset: () =>
                    {
                        logger.LogInformation("Circuit breaker reset. Operations can now proceed.");
                    },
                    onHalfOpen: () =>
                    {
                        logger.LogInformation("Circuit breaker is half-open. Testing operation.");
                    });
        }
    }
}
