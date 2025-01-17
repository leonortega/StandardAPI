namespace StandardAPI.Common.Settings
{
    public class PollySettings
    {
        public int RetryCount { get; set; }
        public int RetryIntervalInSeconds { get; set; }
        public int CircuitBreakerDurationInSeconds { get; set; }
        public int CircuitBreakerExceptionsAllowedBeforeBreaking { get; set; }
        public int RateLimitMaxRequest { get; set; }
        public int RateLimitTimeWindowInSeconds { get; set; }

    }
}
